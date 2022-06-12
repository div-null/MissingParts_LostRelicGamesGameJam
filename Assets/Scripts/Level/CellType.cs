using System;

namespace Level
{
    /// <summary>
    /// Задает тип клетки на уровне
    /// </summary>
    [Serializable]
    public enum CellType
    {
        Wall = 0,
        Floor,
        Pit,
        Finish
    }
}