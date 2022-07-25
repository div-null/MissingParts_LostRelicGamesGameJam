using System.Collections.Generic;
using Assets.Scripts.Field.Cell;

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

                if (finishCell.Container.Part.Color != _finishColor || !HasRightShape(finishCell.Container.Part, visitedParts))
                    return false;
            }

            return true;
        }

        private bool HasRightShape(CharacterPart characterPart, HashSet<CharacterPart> visitedParts)
        {
            bool visitNode(CharacterPart part)
            {
                if (part == null) return true;
                if (visitedParts.Contains(part)) return true;
                visitedParts.Add(part);

                if (_field.TryGet(part.Position, out Cell cell) && !cell.IsFinish())
                    return false;

                return visitNode(part.Down)
                       && visitNode(part.Up)
                       && visitNode(part.Right)
                       && visitNode(part.Left);
            }

            return visitNode(characterPart);
        }
    }
}