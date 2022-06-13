using System;
using UnityEngine;

namespace LevelEditor
{
    [CreateAssetMenu(fileName = "Create New Level", order = 0)]
    [Serializable]
    public class GameLevel
    {
        public CellContainer[][] Cells;
        public int MapHeight = 2;
        public int MapWidth = 2;
        public ColorType FinishColor;
        public PlayerPart[] PlayerParts;
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