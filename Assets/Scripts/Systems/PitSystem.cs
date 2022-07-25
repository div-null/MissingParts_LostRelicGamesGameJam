using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;

namespace Systems
{
    public class PitSystem
    {
        private Field _field;

        private AttachmentSystem _attachmentSystem;

        //unused
        public void CheckForPits(CharacterPart characterPart)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            void visitNode(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                //if (HasPitInCell(part.Position))
                //    _characterPart.PutAway();

                visitNode(part.Left);
                visitNode(part.Right);
                visitNode(part.Up);
                visitNode(part.Down);
            }

            visitNode(characterPart);
        }

        public CharacterPart PreserveMaxPart(CharacterPart target)
        {
            List<CharacterPart> remainingGraphs = RemoveFallenParts(target);

            //Choose max size chain as main character
            (int maxSize, int index) = GetBiggestGraph(remainingGraphs);

            //Other chains will turn inactive
            if (remainingGraphs.Count > 1)
            {
                for (int i = 0; i < remainingGraphs.Count; i++)
                {
                    if (i != index)
                    {
                        remainingGraphs[i].SetActiveToAllParts(false);
                        _attachmentSystem.UpdateLinks(remainingGraphs[i]);
                    }
                }
            }

            return maxSize > 0 ? remainingGraphs[index] : null;
        }

        public CharacterPart PreserveConnectedPart(CharacterPart target, CharacterPart newHead)
        {
            List<CharacterPart> remainingGraphs = RemoveFallenParts(target);

            //Choose max size chain as main character
            CharacterPart mainPart = FindUnitedWithPart(remainingGraphs, newHead);

            foreach (var part in remainingGraphs)
            {
                if (part != mainPart)
                {
                    part.SetActiveToAllParts(false);
                    _attachmentSystem.UpdateLinks(part);
                }
            }

            mainPart.SetActiveToAllParts(true);
            return mainPart;
        }

        private List<CharacterPart> RemoveFallenParts(CharacterPart target)
        {
            //Find all parts in pits
            //Remove their links with other parts
            //Create lists for remaining parts and deleting parts
            List<CharacterPart> remainingParts = GetRemainingAndDeletingParts(target, out List<CharacterPart> deletingParts);

            //delete deleting parts
            foreach (var part in deletingParts)
            {
                _field.Get(part.Position).RemoveCharacterPart(part);
                part.Delete();
            }

            if (remainingParts.Count == 0)
                return remainingParts;

            return GetIsolatedGroups(remainingParts);
        }

        private CharacterPart FindUnitedWithPart(List<CharacterPart> unitedParts, CharacterPart characterPart) =>
            unitedParts.FirstOrDefault(part => FindPart(part, characterPart));

        private List<CharacterPart> GetRemainingAndDeletingParts(CharacterPart characterPart, out List<CharacterPart> deletingParts)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
            List<CharacterPart> remaining = new List<CharacterPart>();
            List<CharacterPart> deleting = new List<CharacterPart>();

            void Separate(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                Separate(part.Down);
                Separate(part.Up);
                Separate(part.Right);
                Separate(part.Left);

                Cell cell = _field.Get(part.Position);
                if (cell.IsPit())
                {
                    deleting.Add(part);
                    part.RemoveLinks();
                }
                else
                    remaining.Add(part);
            }

            Separate(characterPart);
            deletingParts = deleting;
            return remaining;
        }

        private (int size, int index) GetBiggestGraph(List<CharacterPart> unitedParts)
        {
            int maxSize = -1;
            int index = -1;
            for (int i = 0; i < unitedParts.Count; i++)
            {
                int currentSize = CountGraphSize(unitedParts[i]);
                if (currentSize > maxSize)
                {
                    maxSize = currentSize;
                    index = i;
                }
            }

            return (maxSize, index);
        }


        /// <summary>
        /// Returns list of isolated groups of character parts
        /// </summary>
        /// <param name="parts"></param>
        /// <returns>List of character graphs</returns>
        private static List<CharacterPart> GetIsolatedGroups(List<CharacterPart> parts)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
            List<CharacterPart> groups = new List<CharacterPart>();
            foreach (var p in parts)
            {
                if (!visited.Contains(p))
                    groups.Add(p);

                void visitNodes(CharacterPart part)
                {
                    if (part == null) return;
                    if (visited.Contains(part)) return;
                    visited.Add(part);

                    visitNodes(part.Down);
                    visitNodes(part.Up);
                    visitNodes(part.Right);
                    visitNodes(part.Left);
                }

                visitNodes(p);
            }

            return groups;
        }

        private int CountGraphSize(CharacterPart characterPart)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            void visitNode(CharacterPart part)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                visitNode(part.Down);
                visitNode(part.Up);
                visitNode(part.Right);
                visitNode(part.Left);
            }

            visitNode(characterPart);
            return visited.Count;
        }

        private bool FindPart(CharacterPart united, CharacterPart characterPart)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            bool visitNode(CharacterPart part)
            {
                if (part == null) return false;
                if (visited.Contains(part)) return false;
                visited.Add(part);

                if (part == characterPart)
                    return true;
                else
                    return visitNode(part.Down) || visitNode(part.Up) || visitNode(part.Right) || visitNode(part.Left);
            }


            return visitNode(united);
        }
    }
}