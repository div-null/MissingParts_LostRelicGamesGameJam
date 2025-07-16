using System;
using Game.Cell;
using Game.Character;
using JetBrains.Annotations;

namespace LevelEditor
{
    [Serializable]
    public class LevelData
    {
        public int MapHeight;
        public int MapWidth;

        public CellContainer[][] Cells;
        public ColorType FinishColor;
        public CharacterPartData[] PlayerParts;

        public MiscData[] Misc;

        [CanBeNull] public CellContainer Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight)
                return null;

            return Cells[y][x];
        }
    }

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

    [Serializable]
    public class CellContainer
    {
        public CellType Type;
    }

    [Serializable]
    public class MiscData
    {
        public int X;
        public int Y;
        public int Sprite;
    }
}