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
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            bool visitNode(CharacterPart part)
            {
                if (part == null) return true;
                if (visited.Contains(part)) return true;
                visited.Add(part);

                var destination = part.Position + direction.ToVector2Int();
                if (HasWallInCell(destination)) return false;

                return visitNode(part.Left) && visitNode(part.Right) && visitNode(part.Up) && visitNode(part.Down);
            }

            return visitNode(characterPart);
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
                if (HasWallInCell(destination)) return false;

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
                var destination = part.Position + direction.ToVector2Int();
                part.SetPosition(destination);

                visitNode(part.Left);
                visitNode(part.Right);
                visitNode(part.Up);
                visitNode(part.Down);
            }

            visitNode(characterPart);
            return true;
        }

        private bool HasWallInCell(Vector2Int position)
        {
            Cell cell = _field.Get(position);

            if (cell == null)
                return true;

            if (cell.IsWall())
                return true;

            return false;
        }

        private bool HasPitInCell(Vector2Int position)
        {
            Cell cell = _field.Get(position);

            if (cell != null && cell.IsPit())
                return true;

            return false;
        }

        public bool CanThisMove(CharacterPart characterPart, DirectionType direction)
        {
            Vector2Int directionVector = direction.ToVector2Int();
            Cell cell = _field.Get(characterPart.Position + directionVector);
            if (cell.IsWall())
                return false;
            else
                return true;
        }

        public void MoveThis(CharacterPart characterPart, DirectionType direction)
        {
            var destination = characterPart.Position + direction.ToVector2Int();
            characterPart.SetPosition(destination);
        }
    }
}