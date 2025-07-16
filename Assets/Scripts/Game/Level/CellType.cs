using System;

namespace Game.Level
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