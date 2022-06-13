using System;

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