using System;

namespace LevelEditor
{
    /// <summary>
    /// Определяет часть игрока на поле
    /// </summary>
    [Serializable]
    public class PlayerPart
    {
        public int X;
        public int Y;
        public bool IsActive;
        public int Rotation;
        public CellColor Color;
        public PlayerPartType Ability;
        public int Sprite;
    }
}