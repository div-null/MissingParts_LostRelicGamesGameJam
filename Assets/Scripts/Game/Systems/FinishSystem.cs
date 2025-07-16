using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Level;
using Unity.VisualScripting;

namespace Game.Systems
{
    public class FinishSystem
    {
        private readonly Field _field;

        public FinishSystem(Field field)
        {
            _field = field;
        }

        public bool CheckFinished()
        {
            var visitedParts = new HashSet<CharacterPart>();
            foreach (var finishCell in _field.FinishCells)
            {
                if (finishCell.Container == null)
                    return false;

                CharacterPart characterPart = finishCell.Container.Part;

                if (characterPart.Color != _field.FinishColor)
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