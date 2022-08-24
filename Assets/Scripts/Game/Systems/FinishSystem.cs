using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Unity.VisualScripting;

namespace Game.Systems
{
    public class FinishSystem
    {
        private readonly Field _field;
        private List<Cell.Cell> _finishCells;
        private ColorType _finishColor;

        public FinishSystem(Field field)
        {
            _field = field;
        }

        public void Initialize(List<Cell.Cell> finishCells, ColorType finishColor)
        {
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
            _field.TryGet(part.Position, out Cell.Cell cell) && cell.IsFinish();
    }
}