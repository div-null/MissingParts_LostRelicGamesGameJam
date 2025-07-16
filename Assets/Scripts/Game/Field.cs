using System;
using System.Collections.Generic;
using Game.Character;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class Field : IDisposable
    {
        private Cell.Cell[,] _cells;
        private Dictionary<CharacterPart, CharacterPartContainer> _containers = new();
        public Cell.Cell[] FinishCells { get; private set; }
        public ColorType FinishColor { get; private set; }

        public void Initialize(Cell.Cell[,] cells, Cell.Cell[] finishCells, ColorType finishColor)
        {
            _cells = cells;
            FinishCells = finishCells;
            FinishColor = finishColor;
        }

        public Cell.Cell Get(Vector2Int coordinates) =>
            Get(coordinates.x, coordinates.y);

        public Cell.Cell Get(int x, int y)
        {
            if (x < 0 || y < 0 || x > _cells.GetLength(0) - 1 || y > _cells.GetLength(1) - 1)
                return null;
            return _cells[x, y];
        }

        public bool TryGet(int x, int y, out Cell.Cell cell)
        {
            cell = Get(x, y);
            return cell != null;
        }

        public bool TryGet(Vector2Int coordinates, out Cell.Cell cell)
        {
            cell = Get(coordinates);
            return cell != null;
        }

        public void Attach(CharacterPartContainer container, Vector2Int position)
        {
            if (!_containers.TryAdd(container.Part, container))
            {
                Debug.LogWarning("Attempted to register part multiple times", container);
            }
            else
            {
                Cell.Cell cell = Get(position);
                cell.AssignCharacterPart(container);
            }
        }

        public void Replace(CharacterPart part, Vector2Int newPosition)
        {
            Get(part.Position).RemoveCharacterPart(part);
            Get(newPosition).AssignCharacterPart(_containers[part]);
        }

        public void Dispose()
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            for (int i = 0; i < _cells.GetLength(0); i++)
                if (_cells[i, j] != null)
                    Object.Destroy(_cells[i, j].gameObject);
        }
    }
}