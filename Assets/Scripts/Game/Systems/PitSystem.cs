using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Unity.VisualScripting;

namespace Game.Systems
{
    public class PitSystem
    {
        private readonly Field _field;
        private readonly AttachmentSystem _attachmentSystem;

        public PitSystem(Field field, AttachmentSystem attachmentSystem)
        {
            _field = field;
            _attachmentSystem = attachmentSystem;
        }

        public CharacterPart PreserveMaxPart(CharacterPart graph)
        {
            if (!TryRemoveFallenParts(graph, out List<CharacterPart> remainingGraphs))
                return graph;
            
            if (remainingGraphs.Count == 0)
                return null;

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

        public CharacterPart PreserveConnectedPart(CharacterPart graph, CharacterPart newHead)
        {
            if (!TryRemoveFallenParts(graph, out List<CharacterPart> remainingGraphs))
                return graph;

            if (remainingGraphs.Count == 0)
                return null;
            
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

        private bool CheckForPits(CharacterPart characterPart)
        {
            foreach (CharacterPart part in characterPart)
            {
                if (_field.Get(part.Position).IsPit())
                    return true;
            }

            return false;
        }

        private bool TryRemoveFallenParts(CharacterPart target, out List<CharacterPart> graphs)
        {
            //Find all parts in pits
            //Remove their links with other parts
            //Create lists for remaining parts and deleting parts
            if (!GetRemainingAndDeletingParts(target.ToList(), out List<CharacterPart> remainingParts, out List<CharacterPart> deletingParts))
            {
                graphs = new List<CharacterPart>() {target};
                return false;
            }

            //delete parts
            foreach (var part in deletingParts)
            {
                _field.Get(part.Position).RemoveCharacterPart(part);
                part.Delete();
            }

            if (remainingParts.Count == 0)
            {
                graphs = new List<CharacterPart>();
                return true;
            }

            graphs = GetIsolatedGroups(remainingParts);
            return true;
        }

        private CharacterPart FindUnitedWithPart(List<CharacterPart> unitedParts, CharacterPart characterPart) =>
            unitedParts.FirstOrDefault(graph => graph.Contains(characterPart));

        /// <summary>
        /// Gets the remaining and deleting parts of graph
        /// </summary>
        /// <param name="activeParts"></param>
        /// <param name="remainingParts"></param>
        /// <param name="deletingParts"></param>
        /// <returns>Has deleting parts</returns>
        private bool GetRemainingAndDeletingParts(List<CharacterPart> activeParts, out List<CharacterPart> remainingParts, out List<CharacterPart> deletingParts)
        {
            deletingParts = new List<CharacterPart>();
            remainingParts = new List<CharacterPart>();

            foreach (CharacterPart part in activeParts)
            {
                Cell.Cell cell = _field.Get(part.Position);
                if (cell.IsPit())
                {
                    deletingParts.Add(part);
                    part.RemoveLinks();
                }
                else
                    remainingParts.Add(part);
            }

            return deletingParts.Any();
        }

        private (int size, int index) GetBiggestGraph(List<CharacterPart> unitedParts)
        {
            int maxSize = -1;
            int index = -1;
            for (int i = 0; i < unitedParts.Count; i++)
            {
                int currentSize = unitedParts[i].Count();
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
            foreach (var graph in parts)
            {
                if (!visited.Contains(graph))
                    groups.Add(graph);

                visited.AddRange(graph);
            }

            return groups;
        }
    }
}