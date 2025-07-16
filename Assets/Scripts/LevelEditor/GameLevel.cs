using Game;
using Game.Cell;
using Game.Character;
using UnityEngine;

namespace LevelEditor
{
    public class GameLevel
    {
        public int MapHeight => _data.MapHeight;
        public int MapWidth => _data.MapWidth;
        public ColorType FinishColor => _data.FinishColor;
        public CharacterPartData[] PlayerParts => _data.PlayerParts;
        public MiscData[] Misc => _data.Misc;

        public CellData[,] Cells { get; }
        public int FinishCells { get; }

        private readonly LevelData _data;

        public GameLevel(LevelData data)
        {
            _data = data;

            Cells = ConvertCells(data);
            FinishCells = CountFinishCells();
        }

        public CellData? Get(Vector2Int pos)
        {
            return TryGet(pos, out var result) ? result : null;
        }

        public CellData? Get(int x, int y)
        {
            return TryGet(x, y, out var result) ? result : null;
        }

        public bool TryGet(Vector2Int pos, out CellData cell)
        {
            return TryGet(pos.x, pos.y, out cell);
        }

        public bool TryGet(int x, int y, out CellData cell)
        {
            cell = default;

            if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight)
                return false;

            cell = Cells[x, y];
            return true;
        }

        private static CellData[,] ConvertCells(LevelData data)
        {
            CellData[,] cells = new CellData[data.MapWidth, data.MapHeight];

            if (data.Cells == null) return cells;

            for (int y = 0; y < data.MapHeight; y++)
            {
                for (int x = 0; x < data.MapWidth; x++)
                {
                    CellContainer current = data.Get(x, y)!;
                    var right = data.Get(x + 1, y);
                    var left = data.Get(x - 1, y);
                    var up = data.Get(x, y + 1);
                    var down = data.Get(x, y - 1);

                    var border = DirectionType.None;

                    if (right != null && current.Type != right?.Type)
                    {
                        border |= DirectionType.Right;
                    }

                    if (left != null && current.Type != left?.Type)
                    {
                        border |= DirectionType.Left;
                    }

                    if (up != null && current.Type != up?.Type)
                    {
                        border |= DirectionType.Up;
                    }

                    if (down != null && current.Type != down?.Type)
                    {
                        border |= DirectionType.Down;
                    }

                    cells[x, y] = new CellData(current.Type, border);
                }
            }

            return cells;
        }

        private int CountFinishCells()
        {
            int count = 0;
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    var newCell = Get(x, y);

                    if (newCell?.Type == CellType.Finish)
                        count++;
                }
            }

            return count;
        }
    }
}