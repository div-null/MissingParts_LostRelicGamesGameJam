using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using Unity.Mathematics;
using UnityEngine;

public class PullAbility : Ability
{
    [SerializeField]
    private GameObject _hookPrefab;

    private HookView _hookView;
    
    private const int _range = 5;
    private DirectionType _lookDirection;

    public void Initialize(CharacterPart character, Field field, DirectionType direction)
    {
        base.Initialize(character, field);
        _lookDirection = direction;
        _hookView = GetComponentInChildren<HookView>();
    }
    
    public override void Apply()
    {
        _lookDirection = _characterPart.Rotation.ToDirection();
        if (!_characterPart.HasPartInDirection(_lookDirection))
        {
            TryToAttach();
        }
        else
        {
            TryToDetach();
        }
    }

    private bool TryToAttach()
    {
        CharacterPart foundedCharacterPart = null;
        Vector2Int vectorDirection = _lookDirection.ToVector();
        int numberOfSteps;
        for (numberOfSteps = 1; numberOfSteps < _range; numberOfSteps++)
        {
            Cell currentCell = _field.Get(_characterPart.Position + vectorDirection * numberOfSteps);

            if (currentCell == null)
                break;
            else if (currentCell.IsWall())
                break;
            else if (currentCell.CharacterPart != null)
            {
                foundedCharacterPart = currentCell.CharacterPart;
                break;
            }
        }

        _hookView.RunForward(numberOfSteps - 1);
        
        if (foundedCharacterPart == null)
        {
            return false;
        }
        else
        {
            DirectionType oppositeDirection = (-vectorDirection).ToDirection();
            if (!foundedCharacterPart.IsActive)
            {
                if (foundedCharacterPart.CharacterPartMovement )
                //Move foundedCharacterPart several times
                for (int i = 0; i < numberOfSteps; i++)
                {
                    bool isMoved = foundedCharacterPart.CharacterPartMovement.Move(oppositeDirection);
                    _characterPart.CharacterPartAttachment.AttachParts();
                    foundedCharacterPart.CharacterPartAttachment.AttachParts();
                    if (!isMoved || foundedCharacterPart.IsActive)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (_characterPart.GetPartFromDirection(_lookDirection) != foundedCharacterPart)
                {
                    List<CharacterPart> pulledParts = GetPulledCharacterParts(foundedCharacterPart, _lookDirection);
                    foreach (var part in pulledParts)
                    {
                        if (!part.CharacterPartMovement.CanThisMove(oppositeDirection))
                            return false;
                    }
                
                    foreach (var part in pulledParts)
                    {
                        part.CharacterPartMovement.MoveThis(oppositeDirection);
                    }
                    _characterPart.CharacterPartAttachment.AttachParts();
                }
            }
            
            //if he managed to join, then the operation was successful
            if (foundedCharacterPart.IsActive)
                return true;
            else
                return false;
            
            
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
        }
    }

    private bool TryToDetach()
    {
        CharacterPart detachablePart = _characterPart.GetPartFromDirection(_lookDirection);
        Vector2Int oppositeDirection = -_lookDirection.ToVector();
        List<CharacterPart> pulledParts = GetPulledCharacterParts(detachablePart, _lookDirection);
        if (CanPulledPartsMove(pulledParts))
        {
            _hookView.RunForward(1);
            MovePulledParts(pulledParts);
            return true;
        }
        else
        {
            _hookView.RunForward(0);
            return false;
        }
        //try to move pulledParts up and get the result
        //if failed then move mainParts down and get the result
        //if failed then it cant be detached
        //if success then set links between parts (may be we can use JoinInAllDirections and if it cant join then this side will be null?)
    }

    private void MovePulledParts(List<CharacterPart> pulledParts)
    {
        foreach (var part in pulledParts)
            part.CharacterPartMovement.MoveThis(_lookDirection);

        foreach (var part in pulledParts)
            part.TryJoinAllDirections();

        //tryjoin сработал, осталось только выключить, если нет связи
        //нужно проверить, не является ли 

        //Ошибка
        //_characterPart.CharacterPartAttachment.AttachParts();
        
        CharacterPart firstPulledPart = pulledParts.First();
        if (!CanReachPull(firstPulledPart, _characterPart, new List<CharacterPart>()))
        {
            firstPulledPart.SetActiveToAllParts(false);
        }
        
        _characterPart.SetActiveToAllParts(true);
        firstPulledPart.CharacterPartAttachment.DetachParts();
        //check for pits
        //pulledParts.First().CharacterPartAttachment.DetachParts();
        //turn detach parts inactive
        //turn mainParts active
    }

    private bool CanPulledPartsMove(List<CharacterPart> pulledParts)
    {
        foreach (var part in pulledParts)
        {
            if (!part.CharacterPartMovement.CanThisMove(_lookDirection))
                return false;
        }

        return true;
    }

    private List<CharacterPart> GetPulledCharacterParts(CharacterPart detachablePart, DirectionType pullDirection)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
        List<CharacterPart> pulledParts = new List<CharacterPart>();

        void separateNodes(CharacterPart part, DirectionType fromSide)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            if (CanReachPull(part, _characterPart, pulledParts))
            {
                if (fromSide == pullDirection)
                {
                    pulledParts.Add(part);
                    DirectionType[] oppositeSide = pullDirection.GetPerpendicularDirections();
                    separateNodes(part.GetPartFromDirection(oppositeSide[0]), oppositeSide[0]);
                    separateNodes(part.GetPartFromDirection(oppositeSide[1]), oppositeSide[1]);
                    separateNodes(part.GetPartFromDirection(pullDirection), pullDirection);
                }
                //check for sides but not the opposide side, may be with flag
            }
            else
            {
                pulledParts.Add(part);
                separateNodes(part.Down, pullDirection);
                separateNodes(part.Up, pullDirection);
                separateNodes(part.Right, pullDirection);
                separateNodes(part.Left, pullDirection);
            }
        }

        separateNodes(detachablePart, pullDirection);
        return pulledParts;
    }

    private bool CanReachPull(CharacterPart characterPart, CharacterPart desiredPart, List<CharacterPart> pulledParts)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
        bool DetourToPull(CharacterPart part, CharacterPart desiredPart)
        {
            if (part == null) return false;
            if (visited.Contains(part) || pulledParts.Contains(part)) return false;
            visited.Add(part);

            if (part == desiredPart)
                return true;

            return DetourToPull(part.Down, desiredPart) ||
                   DetourToPull(part.Up, desiredPart) ||
                   DetourToPull(part.Right, desiredPart) ||
                   DetourToPull(part.Left, desiredPart);
        }

        return DetourToPull(characterPart, desiredPart);
    }

    //TryToAttach: Is there a characterPart on the field in this direction within 4 cells? If not, then nothing can be attached.
    //TryToDetach: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
    // Move all parts from up, that have connections with this special block Like: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
    // Detach them
}