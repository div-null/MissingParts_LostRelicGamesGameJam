using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputListener : MonoBehaviour
{
    private Character _character;
    private PlayerInputs _playerInputs;

    public void Initialize(PlayerInputs inputs, Character character)
    {
        _character = character;
        _playerInputs = inputs;
        _playerInputs.CharacterControls.Movement.performed += Move_performed;
        _playerInputs.CharacterControls.Select.performed += Select_performed;
    }

    private void Select_performed(InputAction.CallbackContext obj)
    {
        TryToApplyAbility(Mouse.current.position.ReadValue());
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        Vector2 receivedDirection = obj.ReadValue<Vector2>();
        if (receivedDirection != Vector2.zero)
            _character.Move(receivedDirection);
    }

    private void OnDestroy()
    {
        _playerInputs.CharacterControls.Movement.performed -= Move_performed;
        _playerInputs.CharacterControls.Select.performed -= Select_performed;
        
    }

    private void TryToApplyAbility(Vector2 mousePosition)
    {
        Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePositionInWorld, Vector2.zero);
        if (hit.transform != null) {
            Transform objectHit = hit.transform;
            Debug.Log($"hitted something: {objectHit}");
            Ability ability;
            if (objectHit.TryGetComponent<Ability>(out ability))
                ability.Apply();
        }
    }

    public void LockInputs()
    {
        _playerInputs.Disable();
    }

    public void UnlockInputs()
    {
        _playerInputs.Enable();
    }
}
