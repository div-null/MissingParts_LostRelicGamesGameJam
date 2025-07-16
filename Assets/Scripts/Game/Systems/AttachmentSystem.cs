using Game.Character;
using Game.Level;
using UnityEngine;

namespace Game.Systems
{
    public class AttachmentSystem
    {
        private readonly Field _field;

        public AttachmentSystem(Field field)
        {
            _field = field;
        }

        /// <summary>
        /// Establishes links for whole body
        /// </summary>
        /// <param name="graph"></param>
        public void UpdateLinks(CharacterPart graph)
        {
            foreach (CharacterPart part in graph)
                if (part.IsLeaf())
                    UpdatePartLinks(part);
            
            Debug.Log("Part links updated");
        }


        /// <summary>
        /// Establishes links for one body part
        /// </summary>
        /// <param name="part"></param>
        public void UpdatePartLinks(CharacterPart part)
        {
            void UpdateLink(CharacterPart p, DirectionType direction)
            {
                Vector2Int checkPosition = p.Position + direction.ToVector2Int();
                if (_field.TryGet(checkPosition, out var cell))
                {
                    if (cell.Container != null)
                        p.Join(cell.Container.Part, p.IsActive);
                    else
                        p.RemoveLinkInDirection(direction);
                }
            }

            UpdateLink(part, DirectionType.Down);
            UpdateLink(part, DirectionType.Up);
            UpdateLink(part, DirectionType.Left);
            UpdateLink(part, DirectionType.Right);
        }
    }
}