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
    public class LevelFactory : IDisposable
    {
        private readonly IObjectResolver _resolver;
        private readonly GameSettings _gameSettings;
        private readonly List<CharacterPartContainer> _cachedParts = new();

        private readonly AudioManager _audioManager;
        private Field _field;
        private CharacterController _character;

        private GameObject _cellsContainer;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public CharacterController CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            AttachmentSystem attachmentSystem = _resolver.Resolve<AttachmentSystem>();

            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (CharacterPartData partData in playerParts)
            {
                CharacterPartContainer partContainer = CreateCharacterPart(field, partData);

                attachmentSystem.UpdateLinks(partContainer.Part);

                parts.Add(partContainer.Part);
                _cachedParts.Add(partContainer);
            }


            _character = _resolver.Resolve<CharacterController>();
            _character.Initialize(parts.First(), 4);
            return _character;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell.Cell[level.MapWidth, level.MapHeight];
            var finishCells = new List<Cell.Cell>();
            _cellsContainer = _cellsContainer ? _cellsContainer : GetOrSpawn("Cells");

            DirectionType[,] bordersMap = level.GetCellsBorders();
            _field = _resolver.Resolve<Field>();

            _field.SetCells(cells);
            SetupCamera(level);

            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    Vector2Int cellPosition = new Vector2Int(i, j);
                    var newCell = CreateCell(level, cellPosition, bordersMap, cells);

                    if (newCell.CellType == CellType.Finish)
                        finishCells.Add(newCell);
                }
            }

            InitializeFinish(level, finishCells);
            SpawnInactiveParts(level);

            return _field;
        }

        public void Dispose()
        {
            CleanUp();
        }

        private static GameObject GetOrSpawn(string goName) =>
            GameObject.Find(goName) ?? new GameObject(goName);

        private Cell.Cell CreateCell(GameLevel level, Vector2Int position, DirectionType[,] bordersMap, Cell.Cell[,] cells)
        {
            CellContainer cellData = level.Get(position);
            DirectionType borders = bordersMap[position.x, position.y];
            Cell.Cell newCell = SpawnCell(position.x, position.y, _cellsContainer.transform, cellData);

            if (cellData.Type == CellType.Wall || cellData.Type == CellType.Pit)
            {
                TileView tileView = newCell.GetComponent<TileView>();
                tileView.DrawBorders(borders, position, bordersMap, level.Get);
            }

            newCell.Initialize(position, cellData.Type, borders);

            cells[position.x, position.y] = newCell;
            return newCell;
        }

        private void SpawnInactiveParts(GameLevel level)
        {
            AttachmentSystem attachmentSystem = _resolver.Resolve<AttachmentSystem>();
            var inactiveParts = level.PlayerParts.Where(part => !part.IsActive);
            foreach (CharacterPartData playerPart in inactiveParts)
            {
                CharacterPartContainer partContainer = CreateCharacterPart(_field, playerPart);

                attachmentSystem.UpdateLinks(partContainer.Part);
                _cachedParts.Add(partContainer);
            }
        }

        private void InitializeFinish(GameLevel level, List<Cell.Cell> finishCells)
        {
            FinishSystem finishSystem = _resolver.Resolve<FinishSystem>();
            finishSystem.Initialize(finishCells, level.FinishColor);

            foreach (var cell in finishCells)
                cell.GetComponent<FinishView>().SetColor(level.FinishColor);
        }

        private Cell.Cell SpawnCell(int x, int y, Transform parent, CellContainer cellContainer)
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

        private void CleanUp()
        {
            foreach (CharacterPartContainer part in _cachedParts)
                if (part != null && part.gameObject != null)
                    Object.Destroy(part.gameObject);

            _cachedParts.Clear();
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