using System;

namespace LevelEditor
{
    /// <summary>
    /// Определяет тип клетки игрока
    /// </summary>
    [Serializable]
    public enum PlayerPartType
    {
        Common = 0,
        Rotation = 1,
        Hook = 2
    }
}