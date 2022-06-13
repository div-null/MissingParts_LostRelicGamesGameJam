using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputListener : MonoBehaviour
{
    private Character _character;
    private PlayerInputs _playerInputs;

    // Start is called before the first frame update
    void Start()
    {
        ////TODO: move it to factory
        _playerInputs = new PlayerInputs();
        _playerInputs.Enable();
        _playerInputs.CharacterControls.Movement.performed += Move_performed;
        _playerInputs.CharacterControls.Select.performed += Select_performed;
    }

    private void Select_performed(InputAction.CallbackContext obj)
    {
        TryToApplyAbility(Mouse.current.position);
    }

    private void TryToApplyAbility(Vector2Control mousePosition)
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePosition.ReadValue());
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            Ability ability;
            if (objectHit.TryGetComponent<Ability>(out ability))
                ability.Apply();
        }
    }

    public void Move_performed(InputAction.CallbackContext obj)
    {
        Vector2 receivedDirection = obj.ReadValue<Vector2>();
        if (receivedDirection != Vector2.zero)
            _character.Move(receivedDirection);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
