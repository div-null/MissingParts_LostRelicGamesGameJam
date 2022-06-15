using System;
using UnityEngine;

namespace Assets.Scripts.Field.Cell
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int Position;
        public CellType CellType;
        public CharacterPart CharacterPart;
        [SerializeField]
        private DirectionType _borderDirections;
        public const float CellSize = 1f;

        public void Initialize(Vector2Int position, CellType cellType, DirectionType borderDirections)
        {
            _borderDirections = borderDirections;
            Position = position;
            CellType = cellType;

            Vector3 leftSide = CellSize * 0.5f * Vector3.left;
            Vector3 rightSide = CellSize * 0.5f * Vector3.right;
            Vector3 topSide = CellSize * 0.5f * Vector3.up;
            Vector3 bottomSide = CellSize * 0.5f * Vector3.down;

            Debug.DrawRay(Vector3.zero, Vector3.up);
            Debug.DrawRay(Vector3.zero, Vector3.right);
            
            var center = new Vector3(Position.x, Position.y, -1);
            if (_borderDirections.HasFlag(DirectionType.Up))
                Debug.DrawLine(leftSide + topSide + center, rightSide + topSide + center, Color.blue, Single.PositiveInfinity);
            if (_borderDirections.HasFlag(DirectionType.Down))
                Debug.DrawLine(leftSide + bottomSide + center, rightSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
            if (_borderDirections.HasFlag(DirectionType.Left))
                Debug.DrawLine(leftSide + topSide + center, leftSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
            if (_borderDirections.HasFlag(DirectionType.Right))
                Debug.DrawLine(rightSide + topSide + center, rightSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
        }

        public bool IsWall()
        {
            return CellType == CellType.Wall;
        }

        public bool IsPit()
        {
            return CellType == CellType.Pit;
        }

        public bool IsFinish()
        {
            return CellType == CellType.Finish;
        }

        public bool HasCharacterPart()
        {
            return CharacterPart != null;
        }

        public bool HasActiveCharacterPart()
        {
            return HasCharacterPart() && CharacterPart.IsActive;
        }

        public void RemoveCharacterPart(CharacterPart characterPart)
        {
            if (CharacterPart == characterPart)
                CharacterPart = null;
        }

        public void AssignCharacterPart(CharacterPart characterPart)
        {
            CharacterPart = characterPart;
        }
    }
}