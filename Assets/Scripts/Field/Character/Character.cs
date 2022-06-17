using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class Character : MonoBehaviour
{
    private CharacterPart _mainPart;
    private ColorType _characterColor;
    private Field _field;
    private PlayerInputs _playerInputs;

    public event Action Moved;
    public event Action StartMoving;
    public event Action Died;

    [Inject]
    public void Construct(PlayerInputs playerInputs)
    {
        _playerInputs = playerInputs;
    }

    public void Initialize(Field field)
    {
        _field = field;
    }

    public void AddParts(List<CharacterPart> parts)
    {
        // Взяли первый элемент
        _mainPart = parts.First();
        _playerInputs.CharacterControls.Movement.performed += Move_performed;
        _playerInputs.CharacterControls.Select.performed += Select_performed;
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


    private void Move_performed(InputAction.CallbackContext obj)
    {
        Vector2 receivedDirection = obj.ReadValue<Vector2>();
        if (receivedDirection != Vector2.zero)
            Move(receivedDirection);
    }

    private void Select_performed(InputAction.CallbackContext obj)
    {
        TryToApplyAbility(Mouse.current.position.ReadValue());
    }

    private void TryToApplyAbility(Vector2 mousePosition)
    {
        Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePositionInWorld, Vector2.zero);
        if (hit.transform != null)
        {
            Transform objectHit = hit.transform;
            Debug.Log($"hitted something: {objectHit}");
            Ability ability;
            if (objectHit.TryGetComponent<Ability>(out ability))
            {
                CharacterPart characterPart = objectHit.GetComponent<CharacterPart>();
                if (characterPart.IsActive)
                {
                    _mainPart = characterPart;
                    ability.Apply();
                    _mainPart.CharacterPartAttachment.DetachParts();
                    Moved?.Invoke();
                }
            }
        }
    }

    private void OnDestroy()
    {
        _playerInputs.CharacterControls.Movement.performed -= Move_performed;
        _playerInputs.CharacterControls.Select.performed -= Select_performed;
    }
}