using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using UnityEngine;

namespace Systems
{
    public class PullSystem
    {
        private readonly Field _field;
        private readonly int _range;
        private readonly MoveSystem _moveSystem;
        private readonly AttachmentSystem _attachmentSystem;
        private readonly PitSystem _pitSystem;

        public PullSystem(Field field, int pullRange, MoveSystem moveSystem, AttachmentSystem attachmentSystem, PitSystem pitSystem)
        {
            _field = field;
            _range = pullRange;
            _moveSystem = moveSystem;
            _attachmentSystem = attachmentSystem;
            _pitSystem = pitSystem;
        }

        public void ActivateHook(HookView hookView)
        {
            CharacterPart characterPart = hookView.Part;
            var lookDirection = characterPart.Look;

            if (!characterPart.HasPartInDirection(lookDirection))
                TryToAttach(characterPart, hookView);
            else
                TryToDetach(characterPart, hookView);
        }


        //Attach all connected parts:
        //   1. Try to move all parts to character part
        //   *if it cant be moved because of the walls then return false*
        //   2. Attach all parts to character parts of the character


        //Пока что работает не совсем корректно, так как в ситуации когда притягивается фигура:
        //##
        //#

        //К фигуре с формой:
        // |
        //## <- притягивалка

        //Получается, что должно присоединиться в виде фигуры:
        //##
        //#| 
        //## <- притягивалка
        //Либо:
        //##
        //## <- притягивалка
        // #
        //Поэтому пока что будет проверяться на то, может ли притягиваемая фигура не задевать не только стены, но и другие части персонажа

        //Но что если изменить способ притягивания на:
        //1. Притягиваемая фигура двигается как можно ближе к тому, к чему притягивается
        //2. Если она встречает часть персонажа либо стену, то она останавливается
        //3. Потом происходит attach того, что на стороне персонажа и того, что на стороне притягиваемой фигуры
        private bool TryToAttach(CharacterPart characterPart, HookView hookView)
        {
            DirectionType lookDirection = characterPart.Look;
            DirectionType oppositeDirection = lookDirection.Invert();

            (int numberOfSteps, CharacterPartContainer foundPart) = LookupForPart(characterPart.Position, lookDirection.ToVector2Int());

            if (foundPart == null)
            {
                hookView.RunForward(numberOfBlocks: 1);
                return false;
            }

            hookView.RunForward(numberOfBlocks: numberOfSteps - 1);

            if (!foundPart.Part.IsActive)
                return PullPartAndAttach(characterPart, foundPart.Part, oppositeDirection, numberOfSteps);

            return PullAttachedPart(characterPart, foundPart.Part);
        }

        private bool PullAttachedPart(CharacterPart characterPart, CharacterPart foundedCharacterPart)
        {
            DirectionType lookDirection = characterPart.Look;
            DirectionType oppositeDirection = lookDirection.Invert();

            while (characterPart.GetPartFromDirection(lookDirection) != foundedCharacterPart)
            {
                List<CharacterPart> pulledParts = DivideGraphByDirection(characterPart, foundedCharacterPart, lookDirection);

                if (!CanEachPartMove(pulledParts, oppositeDirection))
                    return false;

                foreach (var part in pulledParts)
                    _moveSystem.MovePart(part, oppositeDirection);
                // part.CharacterPartMovement.MoveThis(oppositeDirection);

                _attachmentSystem.UpdateLinks(characterPart);
            }

            return foundedCharacterPart.IsActive;
        }

        //Move foundedCharacterPart several times
        private bool PullPartAndAttach(CharacterPart characterPart, CharacterPart foundedCharacterPart, DirectionType direction, int numberOfSteps)
        {
            for (int i = 0; i < numberOfSteps; i++)
            {
                bool isMoved = _moveSystem.Move(foundedCharacterPart, direction);
                _attachmentSystem.UpdateLinks(characterPart);
                _attachmentSystem.UpdateLinks(foundedCharacterPart);
                if (!isMoved)
                    return foundedCharacterPart.IsActive;
            }

            return false;
        }

        private (int, CharacterPartContainer) LookupForPart(Vector2Int startPosition, Vector2Int vectorDirection)
        {
            for (int numberOfSteps = 1; numberOfSteps <= _range; numberOfSteps++)
            {
                Cell currentCell = _field.Get(startPosition + vectorDirection * numberOfSteps);

                if (currentCell == null || currentCell.IsWall())
                    return (-1, null);

                if (currentCell.Container != null)
                    return (numberOfSteps, currentCell.Container);
            }

            return (-1, null);
        }

        private bool TryToDetach(CharacterPart hookPart, HookView hookView)
        {
            DirectionType lookDirection = hookPart.Look;
            DirectionType oppositeDirection = lookDirection.Invert();
            CharacterPart detachablePart = hookPart.GetPartFromDirection(lookDirection);

            List<CharacterPart> pulledParts = DivideGraphByDirection(hookPart, detachablePart, lookDirection);
            if (CanEachPartMove(pulledParts, oppositeDirection))
            {
                hookView.RunForward(1);
                MovePulledParts(pulledParts, lookDirection);
                DetachGraph(hookPart, pulledParts.First());

                return true;
            }
            else
            {
                hookView.RunForward(0);
                return false;
            }
            //try to move pulledParts up and get the result
            //if failed then move mainParts down and get the result
            //if failed then it cant be detached
            //if success then set links between parts (may be we can use JoinInAllDirections and if it cant join then this side will be null?)
        }

        private void DetachGraph(CharacterPart sourceGraph, CharacterPart detachedGraph)
        {
            //check for pits
            //turn detach parts inactive
            //turn mainParts active
            if (!CanReachSource(sourceGraph, detachedGraph, new List<CharacterPart>()))
                detachedGraph.SetActiveToAllParts(false);

            sourceGraph.SetActiveToAllParts(true);
            _pitSystem.PreserveMaxPart(detachedGraph);
        }

        private void MovePulledParts(List<CharacterPart> pulledParts, DirectionType lookDirection)
        {
            foreach (var part in pulledParts)
                _moveSystem.MovePart(part, lookDirection);

            foreach (var part in pulledParts)
                _attachmentSystem.UpdatePartLinks(part);

            //Ошибка
            //_characterPart.CharacterPartAttachment.AttachParts();
        }

        private bool CanEachPartMove(List<CharacterPart> parts, DirectionType moveDirection) =>
            parts.All(part => _moveSystem.CanPartMove(part, moveDirection));

        private List<CharacterPart> DivideGraphByDirection(CharacterPart sourceGraph, CharacterPart detachablePart, DirectionType detachDirection)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
            List<CharacterPart> detachedParts = new List<CharacterPart>();

            void separateNodes(CharacterPart part, DirectionType fromSide)
            {
                if (part == null) return;
                if (visited.Contains(part)) return;
                visited.Add(part);

                if (CanReachSource(sourceGraph, part, detachedParts))
                {
                    if (fromSide == detachDirection)
                    {
                        detachedParts.Add(part);
                        DirectionType[] oppositeSide = detachDirection.GetPerpendicularDirections();
                        separateNodes(part.GetPartFromDirection(oppositeSide[0]), oppositeSide[0]);
                        separateNodes(part.GetPartFromDirection(oppositeSide[1]), oppositeSide[1]);
                        separateNodes(part.GetPartFromDirection(detachDirection), detachDirection);
                    }
                    //check for sides but not the opposide side, may be with flag
                }
                else
                {
                    detachedParts.Add(part);
                    separateNodes(part.Down, detachDirection);
                    separateNodes(part.Up, detachDirection);
                    separateNodes(part.Right, detachDirection);
                    separateNodes(part.Left, detachDirection);
                }
            }

            separateNodes(detachablePart, detachDirection);
            return detachedParts;
        }

        private bool CanReachSource(CharacterPart sourceGraph, CharacterPart characterPart, List<CharacterPart> detachedParts)
        {
            HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

            bool WalkToSource(CharacterPart part)
            {
                if (part == null) return false;
                if (visited.Contains(part) || detachedParts.Contains(part)) return false;
                visited.Add(part);

                if (part == sourceGraph)
                    return true;

                return WalkToSource(part.Down)
                       || WalkToSource(part.Up)
                       || WalkToSource(part.Right)
                       || WalkToSource(part.Left);
            }

            return WalkToSource(characterPart);
        }

        //TryToAttach: Is there a characterPart on the field in this direction within 4 cells? If not, then nothing can be attached.
        //TryToDetach: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
        // Move all parts from up, that have connections with this special block Like: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
        // Detach them
    }
}