using System;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "Create New Level", order = 0)]
    [Serializable]
    public class GameLevel : ScriptableObject
    {
        public CellType[,] Cells;
        public int MapHeight = 2;
        public int MapWidth = 2;
        public CellColor FinishColor;
        public PlayerPart[] PlayerParts;
    }
}