using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

namespace Systems
{
    public class RotationSystem
    {
        private readonly Field _field;
        private readonly AttachmentSystem _attachmentSystem;
        private MoveSystem _moveSystem;

        public RotationSystem(Field field,MoveSystem moveSystem, AttachmentSystem attachmentSystem)
        {
            _moveSystem = moveSystem;
            _attachmentSystem = attachmentSystem;
            _field = field;
        }

        public bool TryToRotate(CharacterPart characterPart)
        {
            //Up -> Right, Right -> Down, Down -> Left, Left -> Up
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
            Dictionary<CharacterPart, Vector2Int> partsWithNewPositions = new Dictionary<CharacterPart, Vector2Int>();

            bool canRotate(CharacterPart movingPart, Vector2Int rotationCenter)
            {
                if (movingPart == null || visited.Contains(movingPart)) return true;
                visited.Add(movingPart);

                Vector2Int newPosition = RotatePoint(movingPart.Position, rotationCenter);
                partsWithNewPositions.Add(movingPart, newPosition);

                if (CellBlocked(newPosition)) return false;

                return canRotate(movingPart.Left, rotationCenter) &&
                       canRotate(movingPart.Right, rotationCenter) &&
                       canRotate(movingPart.Up, rotationCenter) &&
                       canRotate(movingPart.Down, rotationCenter);
            }

            if (!canRotate(characterPart, characterPart.Position))
                return false;

            foreach ((CharacterPart part, Vector2Int position) in partsWithNewPositions)
            {
                _moveSystem.MovePart(part, position);
                part.Rotate();
            }

            _attachmentSystem.UpdateLinks(characterPart);

            return true;
        }

        private bool CellBlocked(Vector2Int newPosition)
        {
            if (!_field.TryGet(newPosition, out Cell cell))
                return true;

            if (cell.IsWall() || cell.Container.Part is {IsActive: false})
                return true;

            return false;
        }

        private static Vector2Int RotatePoint(Vector2Int point, Vector2Int rotationCenter)
        {
            //d = p2 - p1
            //x1 + dy, y1 - dx
            Vector2Int relativeCoordinates = point - rotationCenter;
            Vector2Int newRelativePosition = new Vector2Int(relativeCoordinates.y, -relativeCoordinates.x);
            Vector2Int newPosition = newRelativePosition + rotationCenter;
            return newPosition;
        }
    }
}