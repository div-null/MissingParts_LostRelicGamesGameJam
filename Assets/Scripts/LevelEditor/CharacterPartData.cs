using System;
using Game.Character;

namespace LevelEditor
{
    /// <summary>
    /// Определяет часть игрока на поле
    /// </summary>
    [Serializable]
    public class CharacterPartData
    {
        public int X;
        public int Y;
        public bool IsActive;
        public int Rotation;
        public ColorType Color;
        public AbilityType Ability;
        public int Sprite;
    }
}