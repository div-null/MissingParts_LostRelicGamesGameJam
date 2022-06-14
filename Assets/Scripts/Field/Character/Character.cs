using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterPart _mainPart;
    private ColorType _characterColor;
    private Field _field;

    public event Action Moved;
    public event Action StartMoving;
    public event Action Died;

    public void Initialize(Field field)
    {
        _field = field;
    }
    
    public void AddParts(List<CharacterPart> parts)
    {
        // Взяли первый элемент
        _mainPart = parts.First();
    }
    
    public void DestroyCharacter()
    {
        _mainPart = null;
        Destroy(this.gameObject);
        //may be something else with events
    }

    public void Move(Vector2 vectorDirection)
    {
        StartMoving?.Invoke();
        _mainPart.CharacterPartMovement.Move(vectorDirection.ToDirection());
        CharacterPart newMainPart = _mainPart.CharacterPartAttachment.DetachParts();
        
        if (newMainPart == null)
        {
            Debug.Log("Game Over!");
            Died?.Invoke();
        }
        else
        {
            _mainPart = newMainPart;
        }
        
        _mainPart.CharacterPartAttachment.AttachParts();
        Moved?.Invoke();
    }
}