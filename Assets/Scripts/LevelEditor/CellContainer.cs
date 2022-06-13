using System;
using Assets.Scripts.Field.Cell;

namespace LevelEditor
{
    [Serializable]
    public class CellContainer
    {
        public CellType Type;

        public CellContainer(CellType type)
        {
            Type = type;
        }
    }
}