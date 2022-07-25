using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using Unity.VisualScripting;

namespace Systems
{
    public class FinishSystem
    {
        private readonly List<Cell> _finishCells;
        private readonly ColorType _finishColor;
        private readonly Field _field;

        public FinishSystem(Field field, List<Cell> finishCells, ColorType finishColor)
        {
            _field = field;
            _finishCells = finishCells;
            _finishColor = finishColor;
        }

        public bool CheckFinished()
        {
            HashSet<CharacterPart> visitedParts = new HashSet<CharacterPart>();
            foreach (var finishCell in _finishCells)
            {
                if (finishCell.Container == null)
                    return false;

                CharacterPart characterPart = finishCell.Container.Part;

                if (characterPart.Color != _finishColor)
                    return false;
                
                if (!visitedParts.Contains(characterPart) && !HasRightShape(characterPart, visitedParts))
                    return false;
            }

            return true;
        }

        private bool HasRightShape(CharacterPart characterPart, HashSet<CharacterPart> visitedParts)
        {
            List<CharacterPart> parts = characterPart.ToList();
            visitedParts.AddRange(parts);
            return parts.All(InsideFinish);
        }

        private bool InsideFinish(CharacterPart part) =>
            _field.TryGet(part.Position, out Cell cell) && cell.IsFinish();
    }
}