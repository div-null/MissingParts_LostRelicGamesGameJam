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
    }

    [Serializable]
    public class Misc
    {
        public int X;
        public int Y;
        public int Sprite;
    }
}