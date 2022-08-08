using System;
using System.Collections.Generic;
using System.Linq;
using Game.Cell;
using Game.Character;
using Game.Systems;
using LevelEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CharacterController = Game.Character.CharacterController;
using Object = UnityEngine.Object;

namespace Game
{
    public class LevelFactory
    {
        private IObjectResolver _resolver;
        private GameSettings _gameSettings;
        private AudioManager _audioManager;
        private List<CharacterPartContainer> _cachedParts = new();
        private Field _field;
        private Character.CharacterController _character;
        private AttachmentSystem? _attachmentSystem;
        private PitSystem _pitSystem;
        private FinishSystem _finishSystem;
        private MoveSystem _moveSystem;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public Character.CharacterController CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (var partData in playerParts)
            {
                CharacterPartContainer partContainer = CreateCharacterPart(field, partData);

                _attachmentSystem.UpdateLinks(partContainer.Part);

                parts.Add(partContainer.Part);
                _cachedParts.Add(partContainer);
            }


            _character = new CharacterController(parts.First(), _resolver.Resolve<PlayerInputs>(), field, _pitSystem, _finishSystem);
            return _character;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell.Cell[level.MapWidth, level.MapHeight];
            var finishCells = new List<Cell.Cell>();

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
                    Cell.Cell newCell = CreateCell(i, j, _field.transform, cellData);

                    if (cellData.Type == CellType.Wall || cellData.Type == CellType.Pit)
                    {
                        TileView tileView = newCell.GetComponent<TileView>();
                        tileView.DrawBorders(borders, cellPosition, bordersMap, level.Get);
                    }

                    newCell.Initialize(cellPosition, cellData.Type, borders);

                    if (newCell.CellType == CellType.Finish)
                        finishCells.Add(newCell);

                    cells[i, j] = newCell;
                }
            }

            _attachmentSystem = new AttachmentSystem(_field);
            _pitSystem = new PitSystem(_field, _attachmentSystem);
            _finishSystem = new FinishSystem(_field, finishCells, level.FinishColor);
            _moveSystem = new MoveSystem(_field);

            foreach (var cell in finishCells)
                cell.GetComponent<FinishView>().SetColor(level.FinishColor);


            var inactiveParts = level.PlayerParts.Where(part => !part.IsActive);
            foreach (var playerPart in inactiveParts)
            {
                CharacterPartContainer partContainer = CreateCharacterPart(_field, playerPart);
                CharacterPart part = partContainer.Part;

                _attachmentSystem.UpdateLinks(part);
                _cachedParts.Add(partContainer);
            }

            return _field;
        }

        public void CleanUp()
        {
            _field.Destroy();
            _character.Dispose();

            foreach (CharacterPartContainer part in _cachedParts)
                if (part != null && part.gameObject != null)
                    Object.Destroy(part.gameObject);

            _cachedParts.Clear();
        }


        private Cell.Cell CreateCell(int x, int y, Transform parent, CellContainer cellContainer)
        {
            Vector3 cellPosition = new Vector3(x, y, -1);
            Cell.Cell cell = cellContainer.Type switch
            {
                CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Pit => _resolver.Instantiate(_gameSettings.PitCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, cellPosition, Quaternion.identity, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(cellContainer.Type), "Unknown cell type")
            };
            return cell;
        }

        private CharacterPartContainer CreateCharacterPart(Field field, CharacterPartData partData)
        {
            Vector3 partPosition = field.Get(partData.X, partData.Y).gameObject.transform.position - Vector3.forward;
            var position = new Vector2Int(partData.X, partData.Y);

            CharacterPartContainer partContainer = _resolver.Instantiate(_gameSettings.CharacterPartPrefab, partPosition, Quaternion.identity);

            partContainer.Part = new CharacterPart(
                position,
                partData.IsActive,
                DirectionFromAngle(partData.Rotation),
                partData.Color,
                partData.Ability);

            field.Attach(partContainer, position);

            if (partData.Ability == AbilityType.Hook)
            {
                var renderer = partContainer.transform.GetComponentInChildren<SpriteRenderer>();
                HookView hookView = _resolver.Instantiate(_gameSettings.HookPrefab, renderer.transform);
                hookView.Initialize(partContainer.Part);
                partContainer.HookView = hookView;
            }

            partContainer.PartView.Initialize(partContainer.Part, partData.Sprite);

            return partContainer;
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
                -90 => DirectionType.Left,
                270 => DirectionType.Left,
                180 => DirectionType.Down,
                -180 => DirectionType.Down,
                90 => DirectionType.Right,
                -270 => DirectionType.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(partRotation), partRotation, "Wrong rotation angle")
            };
        }
    }
}