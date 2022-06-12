using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterPart))]
public abstract class Ability: MonoBehaviour
{
    protected CharacterPart _characterPart;
    public abstract void Apply();

    public void Start()
    {
        _characterPart = this.GetComponent<CharacterPart>();
    }
}
