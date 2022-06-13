using System;

namespace Level
{
    /// <summary>
    /// Определяет тип клетки игрока
    /// </summary>
    [Serializable]
    public enum PlayerPartType
    {
        Common = 0,
        Rotation,
        Hook
    }
}