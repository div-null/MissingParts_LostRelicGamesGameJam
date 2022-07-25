using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using UnityEngine;

namespace Systems
{
    public class AttachmentSystem
    {
        private readonly Field _field;

        public AttachmentSystem(Field field)
        {
            _field = field;
        }

        public void UpdateLinks(CharacterPart characterPart)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            void visitNode(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                visitNode(part.Left);
                visitNode(part.Right);
                visitNode(part.Up);
                visitNode(part.Down);

                if (part.IsLeaf())
                    UpdatePartLinks(part);
            }

            visitNode(characterPart);
        }


        public void UpdatePartLinks(CharacterPart part)
        {
            void UpdateLink(CharacterPart p, DirectionType direction)
            {
                Vector2Int checkPosition = p.Position + direction.ToVector2Int();
                CharacterPart characterPart = _field.Get(checkPosition)?.Container.Part;

                if (characterPart != null)
                    p.Join(characterPart, p.IsActive);
                else
                    p.RemoveLinkInDirection(direction);
            }

            UpdateLink(part, DirectionType.Down);
            UpdateLink(part, DirectionType.Up);
            UpdateLink(part, DirectionType.Left);
            UpdateLink(part, DirectionType.Right);
        }
    }
}