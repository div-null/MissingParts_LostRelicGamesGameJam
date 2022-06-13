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
        public CellColor FinishColor;
        public PlayerPart[] PlayerParts;
        public Misc[] Misc;
        public GameLevel()
        {
            PlayerParts = Array.Empty<PlayerPart>();
            Reset();
        }
        
        private void Reset()
        {
            Cells = new CellContainer[MapHeight][];
            for (int j = 0; j < MapHeight; j++)
            {
                Cells[j] = new CellContainer[MapWidth];
                for (int i = 0; i < MapWidth; i++)
                {
                    Cells[j][i] = new CellContainer(CellType.Empty);
                }
            }
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