using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
    public event Action AppliedPullAbility;
    public event Action AppliedRotateAbility;
    public event Action StartMoving;
    public event Action Died;

    private Vector3 startPosition;
    private float startTime;

    private float minDistance = 4.5f;
    private float maxTime = 0.25f;
    private float directionThreshold = 0.9f;

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
        /*_playerInputs.CharacterControls.PrimaryContact.started += StartTouchPrimary;
        _playerInputs.CharacterControls.PrimaryContact.canceled += EndTouchPrimary;*/
    }

    /*private void StartTouchPrimary(InputAction.CallbackContext obj)
    {
        startTime = (float) obj.startTime;
        Vector2 touchPosition = _playerInputs.CharacterControls.PramaryPosition.ReadValue<Vector2>();
        startPosition = Camera.main.ScreenToWorldPoint(touchPosition);
    }*/

    /*private void EndTouchPrimary(InputAction.CallbackContext obj)
    {
        float endTime = (float)obj.time;
        Vector2 touchPosition = _playerInputs.CharacterControls.PramaryPosition.ReadValue<Vector2>();
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(touchPosition);

        float deltaTime = endTime - startTime;
        if (Vector3.Distance(startPosition, endPosition) >= minDistance && deltaTime < maxTime)
        {
            Vector3 deltaDirection = endPosition - startPosition;
            Vector2 normalizedDirection = new Vector2(deltaDirection.x, deltaDirection.y).normalized;
            if (Vector2.Dot(Vector2.up, normalizedDirection) > directionThreshold)
            {
                Move(Vector2Int.up);
            }
            else if (Vector2.Dot(Vector2.left, normalizedDirection) > directionThreshold)
            {
                Move(Vector2Int.left);
            }
            else if (Vector2.Dot(Vector2.down, normalizedDirection) > directionThreshold)
            {
                Move(Vector2Int.down);
            }
            else if (Vector2.Dot(Vector2.right, normalizedDirection) > directionThreshold)
            {
                Move(Vector2Int.right);
            }
                
            Debug.Log($"yes {Vector3.Distance(startPosition, endPosition)} > {minDistance} && {deltaTime} < {maxTime}");
        }
        else
        {
            Debug.Log($"no {Vector3.Distance(startPosition, endPosition)} > {minDistance} && {deltaTime} < {maxTime}");
        }
    }*/

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
        var newMainPart = _mainPart.CharacterPartAttachment.DetachParts();

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
                    var newMainPart = _mainPart.CharacterPartAttachment.DetachParts(_mainPart);

                    //if (result)
                    //    Detached.Invoke();
                    //
                    _mainPart = newMainPart;
                    if (ability.GetType() == typeof(PullAbility))
                        AppliedPullAbility?.Invoke();
                    else
                        AppliedRotateAbility?.Invoke();
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