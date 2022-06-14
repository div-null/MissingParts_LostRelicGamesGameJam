using System;

namespace Assets.Scripts.Field.Cell
{
    [Serializable]
    public enum CellType
    {
        Empty = 0,
        Wall = 1,
        Pit,
        Finish
    }
}