using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterPart))]
public abstract class Ability: MonoBehaviour
{
    protected CharacterPart _characterPart;
    protected Field _field;
    
    public abstract void Apply();

    public virtual void Initialize(CharacterPart character, Field field)
    {
        _field = field;
        _characterPart = character;
    }
}
