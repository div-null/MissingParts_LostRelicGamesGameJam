using System;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "Create New Level", order = 0)]
    [Serializable]
    public class GameLevel : ScriptableObject
    {
        public CellType[][] Cells;
        public int MapHeight = 2;
        public int MapWidth = 2;
        public CellColor FinishColor;
        public PlayerPart[] PlayerParts;

        private void Reset()
        {
            Cells = new CellType[MapHeight][];
            for (int j = 0; j < Cells.GetLength(1); j++)
            {
                Cells[j] = new CellType[MapWidth];
                for (int i = 0; i < Cells.GetLength(0); i++)
                {
                    Cells[j][i] = CellType.Floor;
                }
            }
        }
    }
}