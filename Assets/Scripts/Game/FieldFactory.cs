using System;
using Game.Cell;
using LevelEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class FieldFactory
    {
        private const float CellsLayer = -1;

        private readonly IObjectResolver _resolver;
        private readonly GameSettings _gameSettings;

        private Field _field;
        private GameObject _cellsContainer;

        public FieldFactory(IObjectResolver resolver, GameSettings gameSettings)
        {
            _resolver = resolver;
            _gameSettings = gameSettings;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell.Cell[level.MapWidth, level.MapHeight];
            var finishCells = new Cell.Cell[level.FinishCells];

            _cellsContainer ??= GetOrSpawn("Cells");

            _field = _resolver.Resolve<Field>();

            int finishCount = 0;

            for (int y = 0; y < level.MapHeight; y++)
            {
                for (int x = 0; x < level.MapWidth; x++)
                {
                    var cellPosition = new Vector2Int(x, y);
                    var newCell = CreateCell(level, cellPosition, cells);

                    if (newCell.CellType == CellType.Finish)
                    {
                        finishCells[finishCount] = newCell;
                        finishCount++;
                    }
                }
            }

            _field.Initialize(cells, finishCells, level.FinishColor);

            return _field;
        }

        private Cell.Cell CreateCell(GameLevel level, Vector2Int position, Cell.Cell[,] cells)
        {
            CellData current = level.Get(position).GetValueOrDefault();
            Cell.Cell newCell = SpawnCell(position.x, position.y, _cellsContainer.transform, current.Type);

            if (current.Type == CellType.Wall || current.Type == CellType.Pit)
            {
                TileView tileView = newCell.GetComponent<TileView>();
                var up = level.Get(position + Vector2Int.up);
                var down = level.Get(position + Vector2Int.down);
                var right = level.Get(position + Vector2Int.right);
                var left = level.Get(position + Vector2Int.left);

                tileView.DrawBorders(current, up, down, right, left, position);
            }

            newCell.Initialize(position, current.Type, current.Borders);

            cells[position.x, position.y] = newCell;
            return newCell;
        }

        private Cell.Cell SpawnCell(int x, int y, Transform parent, CellType type)
        {
            Vector3 cellPosition = new Vector3(x, y, CellsLayer);
            Cell.Cell cell = type switch
            {
                    CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, cellPosition, Quaternion.identity, parent),
                    CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, cellPosition, Quaternion.identity, parent),
                    CellType.Pit => _resolver.Instantiate(_gameSettings.PitCellPrefab, cellPosition, Quaternion.identity, parent),
                    CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, cellPosition, Quaternion.identity, parent),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), "Unknown cell type")
            };
            return cell;
        }

        private static GameObject GetOrSpawn(string goName) =>
                GameObject.Find(goName) ?? new GameObject(goName);

    }
}