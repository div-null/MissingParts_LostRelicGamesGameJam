using System;
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
    }

    [Serializable]
    public class Misc
    {
        public int X;
        public int Y;
        public int Sprite;
    }
}