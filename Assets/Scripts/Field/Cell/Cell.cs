using System;
using UnityEngine;

namespace Assets.Scripts.Field.Cell
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int Position;
        public CellType CellType;
        public CharacterPartContainer Container;

        [field: SerializeField] public DirectionType BorderDirections { get; private set; }
        public const float CellSize = 1f;

        public void Initialize(Vector2Int position, CellType cellType, DirectionType borderDirections)
        {
            BorderDirections = borderDirections;
            Position = position;
            CellType = cellType;

            Vector3 leftSide = CellSize * 0.5f * Vector3.left;
            Vector3 rightSide = CellSize * 0.5f * Vector3.right;
            Vector3 topSide = CellSize * 0.5f * Vector3.up;
            Vector3 bottomSide = CellSize * 0.5f * Vector3.down;

            Debug.DrawRay(Vector3.zero, Vector3.up);
            Debug.DrawRay(Vector3.zero, Vector3.right);

            var center = new Vector3(Position.x, Position.y, -1);
            if (BorderDirections.HasFlag(DirectionType.Up))
                Debug.DrawLine(leftSide + topSide + center, rightSide + topSide + center, Color.blue, Single.PositiveInfinity);
            if (BorderDirections.HasFlag(DirectionType.Down))
                Debug.DrawLine(leftSide + bottomSide + center, rightSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
            if (BorderDirections.HasFlag(DirectionType.Left))
                Debug.DrawLine(leftSide + topSide + center, leftSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
            if (BorderDirections.HasFlag(DirectionType.Right))
                Debug.DrawLine(rightSide + topSide + center, rightSide + bottomSide + center, Color.blue, Single.PositiveInfinity);
        }

        public bool IsWall() =>
            CellType == CellType.Wall;

        public bool IsPit() =>
            CellType == CellType.Pit;

        public bool IsFinish() =>
            CellType == CellType.Finish;

        public bool IsSurface() =>
            CellType == CellType.Empty;

        public bool HasCharacterPart() =>
            Container != null;

        public void RemoveCharacterPart(CharacterPartContainer characterPart)
        {
            if (Container == characterPart)
                Container = null;
        }
        
        public void RemoveCharacterPart(CharacterPart characterPart)
        {
            if (Container.Part == characterPart)
                Container = null;
        }
        

        public void AssignCharacterPart(CharacterPartContainer characterPartContainer)
        {
            Container = characterPartContainer;
        }
    }
}