using System;
using Game;
using Game.Character;
using UnityEngine;

namespace LevelEditor
{
    [Serializable]
    public class GameLevel
    {
        public CellContainer[][] Cells;
        public int MapHeight = 2;
        public int MapWidth = 2;
        public ColorType FinishColor;
        public CharacterPartData[] PlayerParts;
        public Misc[] Misc;

        public CellContainer Get(Vector2Int coords) =>
            Get(coords.x, coords.y);

        public CellContainer Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight)
                return null;
            return Cells[y][x];
        }

        public DirectionType[,] GetCellsBorders()
        {
            DirectionType[,] borders = new DirectionType[MapWidth, MapHeight];
            if (Cells == null) return borders;

            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    CellContainer current = Get(x, y);
                    var rightCell = Get(x + 1, y);
                    var upCell = Get(x, y + 1);
                    if (rightCell != null && current.Type != rightCell.Type)
                    {
                        borders[x, y] = borders[x, y] | DirectionType.Right;
                        borders[x + 1, y] = borders[x + 1, y] | DirectionType.Left;
                    }

                    if (upCell != null && current.Type != upCell.Type)
                    {
                        borders[x, y] = borders[x, y] | DirectionType.Up;
                        borders[x, y + 1] = borders[x, y + 1] | DirectionType.Down;
                    }
                }
            }

            return borders;
        }
    }

    [Serializable]
    public class Misc
    {
        public int X;
        public int Y;
        public int Sprite;
    }
}