using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Assets.Scripts.Field.Cell
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int Position;
        public CellType CellType;
        public CharacterPart CharacterPart;

        public void Initialize(Vector2Int position, CellType cellType)
        {
            Position = position;
            CellType = cellType;
        }

        public bool IsWall()
        {
            return CellType == CellType.Wall;
        }
        
        public bool IsPit()
        {
            return CellType == CellType.Pit;
        }

        public bool HasCharacterPart()
        {
            return CharacterPart != null;
        }
        
        public bool HasActiveCharacterPart()
        {
            return CharacterPart.IsActive;
        }

        public void RemoveCharacterPart()
        {
            CharacterPart = null;
        }

        public void AssignCharacterPart(CharacterPart characterPart)
        {
            CharacterPart = characterPart;
        }
    }
}
