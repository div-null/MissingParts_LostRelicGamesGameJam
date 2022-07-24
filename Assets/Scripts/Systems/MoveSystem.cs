using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

namespace Systems
{
    public class MoveSystem
    {
        private readonly Field _field;

        public MoveSystem(Field field)
        {
            _field = field;
        }

        public bool CanMove(CharacterPart characterPart, DirectionType direction)
        {
            var vector2Int = direction.ToVector2Int();
            return CanMove(characterPart, vector2Int);
        }

        public bool CanMove(CharacterPart characterPart, Vector2Int deltaPosition)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            bool visitNode(CharacterPart part)
            {
                if (part == null) return true;
                if (visited.Contains(part)) return true;
                visited.Add(part);

                var destination = part.Position + deltaPosition;
                if (HasWallIn(destination)) return false;

                return visitNode(part.Left) && visitNode(part.Right) && visitNode(part.Up) && visitNode(part.Down);
            }

            return visitNode(characterPart);
        }

        public bool Move(CharacterPart characterPart, DirectionType direction)
        {
            if (!CanMove(characterPart, direction)) return false;

            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            void visitNode(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                MovePart(part, direction);

                visitNode(part.Left);
                visitNode(part.Right);
                visitNode(part.Up);
                visitNode(part.Down);
            }

            visitNode(characterPart);
            return true;
        }

        public void MovePart(CharacterPart characterPart, DirectionType direction)
        {
            var destination = characterPart.Position + direction.ToVector2Int();
            MovePart(characterPart, destination);
        }

        public void MovePart(CharacterPart characterPart, Vector2Int destination) =>
            SetPosition(characterPart, destination);

        public bool CanPartMove(CharacterPart characterPart, DirectionType direction)
        {
            Vector2Int destination = characterPart.Position + direction.ToVector2Int();
            return !HasWallIn(destination);
        }

        private void SetPosition(CharacterPart part, Vector2Int destination)
        {
            CharacterPartContainer partContainer = _field.Get(part.Position).Container;
            _field.Get(part.Position).RemoveCharacterPart(partContainer);
            _field.Get(destination).AssignCharacterPart(partContainer);
            part.Position = destination;
        }

        private bool HasWallIn(Vector2Int position)
        {
            if (_field.TryGet(position, out Cell cell))
                return cell.IsWall();

            return false;
        }
    }
}