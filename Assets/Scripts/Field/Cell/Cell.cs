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

        public void Initialize(Vector2Int position, CellType cellType, CharacterPart characterPart)
        {
            Position = position;
            CellType = cellType;
            CharacterPart = characterPart;
        }
         
    }
}
