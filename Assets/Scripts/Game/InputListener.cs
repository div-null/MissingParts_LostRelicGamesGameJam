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
        TryToApplyAbility(Mouse.current.position);
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

    private void TryToApplyAbility(Vector2Control mousePosition)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition.ReadValue());
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            Ability ability;
            if (objectHit.TryGetComponent<Ability>(out ability))
                ability.Apply();
        }
    }
}
