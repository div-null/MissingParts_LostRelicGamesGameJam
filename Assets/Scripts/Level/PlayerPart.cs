using System;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Определяет часть игрока на поле
    /// </summary>
    [Serializable]
    public class PlayerPart : ScriptableObject
    {
        public int X;
        public int Y;
        public bool IsActive;
        public CellColor Color;
        public PlayerPartType Type;
    }
}