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
            var finishCells = new Cell.Cell[level.CountFinishCells()];

            _cellsContainer ??= GetOrSpawn("Cells");

            DirectionType[,] bordersMap = level.GetCellsBorders();
            _field = _resolver.Resolve<Field>();

            int finishCount = 0;

            for (int y = 0; y < level.MapHeight; y++)
            {
                for (int x = 0; x < level.MapWidth; x++)
                {
                    var cellPosition = new Vector2Int(x, y);
                    var newCell = CreateCell(level, cellPosition, bordersMap, cells);

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

        private Cell.Cell SpawnCell(int x, int y, Transform parent, CellContainer cellContainer)
        {
            Vector3 cellPosition = new Vector3(x, y, CellsLayer);
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

        private static GameObject GetOrSpawn(string goName) =>
                GameObject.Find(goName) ?? new GameObject(goName);

    }
}