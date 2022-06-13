using System;

namespace LevelEditor
{
    /// <summary>
    /// Задает тип клетки на уровне
    /// </summary>
    [Serializable]
    public enum CellType
    {
        Wall = 0,
        Empty,
        Pit,
        Finish
    }
}