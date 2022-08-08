using System;
using Game.Cell;
using UnityEngine.Animations;

namespace LevelEditor
{
    [Serializable]
    public class CellContainer
    {
        public CellType Type;
        public int Rotation;
    }
}