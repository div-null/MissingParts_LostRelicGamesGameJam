using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class PullAbility : Ability
{
    private const int _range = 5;
    private Field _field;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Apply()
    {
        if (_characterPart.HasPartInDirection(_characterPart.transform.up.ToDirection()))
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
        Vector2Int vectorDirection = _characterPart.transform.up.ToVector();
        int i;
        for (i = 1; i < _range; i++)
        {
            Cell currentCell = _field.Get(_characterPart.Position + vectorDirection * i);
            if (currentCell.IsWall())
            {
                break;
            }
            else if (currentCell.CharacterPart != null)
            {
                foundedCharacterPart = currentCell.CharacterPart;
                break;
            }
        }

        if (foundedCharacterPart == null)
            return false;
        else
        {
            DirectionType oppositeDirection = (-vectorDirection).ToDirection();
            foundedCharacterPart.CharacterPartMovement.Move(oppositeDirection, i);
            _characterPart.CharacterPartAttachment.AttachParts();
            foundedCharacterPart.CharacterPartAttachment.AttachParts();

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

    //TryToAttach: Is there a characterPart on the field in this direction within 4 cells? If not, then nothing can be attached.
    //TryToDetach: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
    // Move all parts from up, that have connections with this special block Like: Can dettachable part move front? If not, then can main part move back? If not, then it cant be dettached
    // Detach them
}
