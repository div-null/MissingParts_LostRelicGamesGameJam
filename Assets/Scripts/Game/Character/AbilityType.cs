using System;

namespace Game.Character
{
    /// <summary>
    /// Определяет тип клетки игрока
    /// </summary>
    [Serializable]
    public enum AbilityType
    {
        Default = 0,
        Rotation = 1,
        Hook = 2
    }
}