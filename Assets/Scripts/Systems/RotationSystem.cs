using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using UnityEngine;

namespace Systems
{
    public class RotationSystem
    {
        private readonly Field _field;
        private readonly AttachmentSystem _attachmentSystem;
        private readonly MoveSystem _moveSystem;

        public RotationSystem(Field field, MoveSystem moveSystem, AttachmentSystem attachmentSystem)
        {
            _moveSystem = moveSystem;
            _attachmentSystem = attachmentSystem;
            _field = field;
        }

        public bool TryToRotate(CharacterPart graph)
        {
            //Up -> Right, Right -> Down, Down -> Left, Left -> Up
            Dictionary<CharacterPart, Vector2Int> partsWithNewPositions = new Dictionary<CharacterPart, Vector2Int>();
            Vector2Int rotationCenter = graph.Position;
            foreach (CharacterPart part in graph)
            {
                Vector2Int newPosition = RotatePoint(part.Position, rotationCenter);
                if (CellBlocked(newPosition)) return false;
                partsWithNewPositions.Add(part, newPosition);
            }

            foreach ((CharacterPart part, Vector2Int position) in partsWithNewPositions)
            {
                _moveSystem.MovePart(part, position);
                part.Rotate();
            }

            _attachmentSystem.UpdateLinks(graph);

            return true;
        }

        private bool CellBlocked(Vector2Int newPosition)
        {
            if (!_field.TryGet(newPosition, out Cell cell))
                return true;

            if (cell.IsWall() || cell.Container is {Part: {IsActive: false}})
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