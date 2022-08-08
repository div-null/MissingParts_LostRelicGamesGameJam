using Game.Character;
using UnityEngine;

namespace Game.Cell
{
    public class Cell : MonoBehaviour
    {
        public const float CellSize = 1f;
        
        public Vector2Int Position;
        public CellType CellType;
        public CharacterPartContainer? Container;

        [field: SerializeField] public DirectionType BorderDirections { get; private set; }

        public void Initialize(Vector2Int position, CellType cellType, DirectionType borderDirections)
        {
            BorderDirections = borderDirections;
            Position = position;
            CellType = cellType;
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
            if (Container != null && Container.Part == characterPart)
                Container = null;
        }

        public void AssignCharacterPart(CharacterPartContainer characterPartContainer)
        {
            Container = characterPartContainer;
        }

        private void OnDrawGizmos()
        {
            Vector3 leftSide =   CellSize * 0.5f * Vector3.left;
            Vector3 rightSide =  CellSize * 0.5f * Vector3.right;
            Vector3 topSide =    CellSize * 0.5f * Vector3.up;
            Vector3 bottomSide = CellSize * 0.5f * Vector3.down;

            var center = new Vector3(Position.x, Position.y, 0);
            if (BorderDirections.HasFlag(DirectionType.Up))
                Debug.DrawLine(center + leftSide + topSide,    center + rightSide + topSide,    Color.blue);
            if (BorderDirections.HasFlag(DirectionType.Down))
                Debug.DrawLine(center + leftSide + bottomSide, center + rightSide + bottomSide, Color.blue);
            if (BorderDirections.HasFlag(DirectionType.Left))
                Debug.DrawLine(center + leftSide + topSide,    center + leftSide + bottomSide,  Color.blue);
            if (BorderDirections.HasFlag(DirectionType.Right))
                Debug.DrawLine(center + rightSide + topSide,   center + rightSide + bottomSide, Color.blue);
        }
    }
}