using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using LevelEditor;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Game
{
    public class LevelFactory
    {
        private IObjectResolver _resolver;
        private GameSettings _gameSettings;
        private AudioManager _audioManager;
        private List<CharacterPartView> _cachedParts = new();
        private Field _field;
        private Character _character;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public Character CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            _character = _resolver.Instantiate(_gameSettings.CharacterPrefab);
            _character.Initialize(field);
            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (var partData in playerParts)
            {
                CharacterPartView characterPart = CreateCharacterPart(field, partData);
                CharacterPart part = characterPart.Part;
                // characterPart.Initialize(new Vector2Int(part.X, part.Y), true, field, DirectionFromAngle(part.Rotation), part.Color);

                part.CharacterPartAttachment.AttachParts();
                field.Get(partData.X, partData.Y).AssignCharacterPart(part);

                parts.Add(part);
                _cachedParts.Add(characterPart);
            }

            _character.AddParts(parts);
            return _character;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell[level.MapWidth, level.MapHeight];

            DirectionType[,] bordersMap = level.GetCellsBorders();
            _field = _resolver.Instantiate(_gameSettings.FieldPrefab);
            _field.SetCells(cells);
            SetupCamera(level);

            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    CellContainer cellData = level.Get(i, j);
                    DirectionType borders = bordersMap[i, j];
                    Vector2Int cellPosition = new Vector2Int(i, j);
                    Cell newCell = CreateCell(i, j, _field.transform, cellData);

                    if (cellData.Type == CellType.Wall || cellData.Type == CellType.Pit)
                    {
                        TileView tileView = newCell.GetComponent<TileView>();
                        tileView.DrawBorders(borders, cellPosition, bordersMap, level.Get);
                    }

                    newCell.Initialize(cellPosition, cellData.Type, borders);
                    cells[i, j] = newCell;
                }
            }

            var inactiveParts = level.PlayerParts.Where(part => !part.IsActive);
            foreach (var playerPart in inactiveParts)
            {
                CharacterPartView partView = CreateCharacterPart(_field, playerPart);
                CharacterPart part = partView.Part;
                // characterPart.Initialize(new Vector2Int(playerPart.X, playerPart.Y), false, _field, DirectionFromAngle(playerPart.Rotation), playerPart.Color);

                part.CharacterPartAttachment.AttachParts();
                cells[part.Position.x, part.Position.y].AssignCharacterPart(part);
                _cachedParts.Add(partView);
            }

            List<Cell> finishCells = GetFinishCells(cells);
            _field.Setup(finishCells, level.FinishColor);

            return _field;
        }

        public void CleanUp()
        {
            _field.Destroy();
            _character.Destroy();
            
            for (int i = 0; i < _cachedParts.Count; i++)
                Object.Destroy(_cachedParts[i]);
        }


        private Cell CreateCell(int x, int y, Transform parent, CellContainer cellContainer)
        {
            Vector3 cellPosition = new Vector3(x, y, -1);
            Cell cell = cellContainer.Type switch
            {
                CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Pit => _resolver.Instantiate(_gameSettings.PitCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, cellPosition, Quaternion.identity, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(cellContainer.Type), "Unknown cell type")
            };
            return cell;
        }

        private CharacterPartView CreateCharacterPart(Field field, CharacterPartData partData)
        {
            Vector3 partPosition = field.Get(partData.X, partData.Y).gameObject.transform.position - Vector3.forward;
            CharacterPartView partView = _resolver.Instantiate(_gameSettings.CharacterPartPrefab,
                partPosition, Quaternion.identity);

            if (partData.Ability == AbilityType.Hook)
            {
                var renderer = partView.transform.GetComponentInChildren<SpriteRenderer>();
                var hookView = _resolver.Instantiate(_gameSettings.HookPrefab, renderer.transform);
                hookView.transform.rotation = Quaternion.identity;
            }

            var characterPart = new CharacterPart();

            partView.Initialize(characterPart, partData);

            field.Get(partData.X, partData.Y).AssignCharacterPart(characterPart);
            setupAbilities(partView, partData, field, characterPart);

            return partView;
        }

        private List<Cell> GetFinishCells(Cell[,] cells)
        {
            var finishCells = new List<Cell>();

            for (int j = 0; j < cells.GetLength(1); j++)
            {
                for (int i = 0; i < cells.GetLength(0); i++)
                {
                    if (cells[i, j].CellType == CellType.Finish)
                    {
                        finishCells.Add(cells[i, j]);
                    }
                }
            }

            return finishCells;
        }

        private void setupAbilities(CharacterPartView characterPartView, CharacterPartData partData, Field field, CharacterPart characterPart)
        {
            if (partData.Ability == AbilityType.Hook)
            {
                var pullAbility = characterPartView.GetOrAddComponent<PullAbility>();
                pullAbility.Initialize(characterPart, field, DirectionFromAngle(partData.Rotation), _audioManager);
            }
            else if (partData.Ability == AbilityType.Rotation)
            {
                var rotateAbility = characterPartView.GetOrAddComponent<RotateAbility>();
                rotateAbility.Initialize(characterPart, field);
            }
        }

        private void SetupCamera(GameLevel level)
        {
            Camera.main.transform.position =
                new Vector3(level.MapWidth / 2f - 0.5f, level.MapHeight / 2f - 0.5f, -10);
        }

        private static DirectionType DirectionFromAngle(int partRotation)
        {
            return partRotation switch
            {
                0 => DirectionType.Up,
                360 => DirectionType.Up,
                90 => DirectionType.Left,
                -270 => DirectionType.Left,
                180 => DirectionType.Down,
                -180 => DirectionType.Down,
                -90 => DirectionType.Right,
                270 => DirectionType.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(partRotation), partRotation, "Wrong rotation angle")
            };
        }
    }
}