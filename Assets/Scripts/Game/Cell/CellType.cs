﻿using System;

namespace Game.Cell
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