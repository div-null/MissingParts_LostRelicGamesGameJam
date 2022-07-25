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

        public void UpdateLinks(CharacterPart graph)
        {
            foreach (CharacterPart part in graph)
                if (part.IsLeaf())
                    UpdatePartLinks(part);
            
            Debug.Log("Part links updated");
        }


        public void UpdatePartLinks(CharacterPart part)
        {
            void UpdateLink(CharacterPart p, DirectionType direction)
            {
                Vector2Int checkPosition = p.Position + direction.ToVector2Int();
                if (_field.TryGet(checkPosition, out var cell))
                {
                    if (cell.Container != null)
                        p.Join(cell.Container.Part, p.IsActive);
                    // else
                    //     p.RemoveLinkInDirection(direction);
                }
            }

            UpdateLink(part, DirectionType.Down);
            UpdateLink(part, DirectionType.Up);
            UpdateLink(part, DirectionType.Left);
            UpdateLink(part, DirectionType.Right);
        }
    }
}