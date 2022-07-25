using System;
using System.Collections.Generic;
using System.Linq;
using LevelEditor;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class Character : MonoBehaviour
{
    public event Action Moved;
    public event Action AppliedPullAbility;
    public event Action AppliedRotateAbility;
    public event Action StartMoving;
    public event Action Died;

    private CharacterPart _mainPart;

    private PlayerInputs _playerInputs;
    private PullSystem _pullSystem;
    private RotationSystem _rotationSystem;
    private AttachmentSystem _attachmentSystem;
    private MoveSystem _moveSystem;
    private PitSystem _pitSystem;

    [Inject]
    public void Construct(PlayerInputs playerInputs,
        MoveSystem moveSystem,
        AttachmentSystem attachmentSystem,
        RotationSystem rotationSystem,
        PullSystem pullSystem)
    {
        _moveSystem = moveSystem;
        _attachmentSystem = attachmentSystem;
        _rotationSystem = rotationSystem;
        _pullSystem = pullSystem;
        _playerInputs = playerInputs;
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

    public void Destroy()
    {
        _mainPart = null;
        Destroy(this.gameObject);
        //may be something else with events
    }

    private void Move(DirectionType direction)
    {
        StartMoving?.Invoke();
        _moveSystem.Move(_mainPart, direction);
        // check for pits
        var newMainPart = _pitSystem.PreserveMaxPart(_mainPart);

        if (newMainPart == null)
        {
            Debug.Log("Game Over!");
            Died?.Invoke();
        }
        else
        {
            _mainPart = newMainPart;
        }

        _attachmentSystem.UpdateLinks(_mainPart);
        Moved?.Invoke();
    }


    private void TryToApplyAbility(Vector2 mousePosition)
    {
        Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePositionInWorld, Vector2.zero);
        if (hit.transform != null)
        {
            Transform objectHit = hit.transform;
            Debug.Log($"hitted something: {objectHit}");
            if (objectHit.TryGetComponent(out CharacterPartContainer partContainer))
            {
                if (partContainer.Part.IsActive)
                {
                    // _mainPart = partContainer.Part;

                    if (!ApplyAbility(partContainer)) return;

                    _mainPart = _pitSystem.PreserveConnectedPart(_mainPart, partContainer.Part);
                }
            }
        }
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        Vector2 receivedDirection = obj.ReadValue<Vector2>();
        if (receivedDirection != Vector2.zero)
            Move(receivedDirection.ToDirection());
    }

    private void Select_performed(InputAction.CallbackContext obj)
    {
        TryToApplyAbility(Mouse.current.position.ReadValue());
    }

    private bool ApplyAbility(CharacterPartContainer partContainer)
    {
        switch (partContainer.Part.Ability)
        {
            case AbilityType.Rotation:
                _rotationSystem.TryToRotate(partContainer.Part);
                AppliedRotateAbility?.Invoke();
                break;
            case AbilityType.Hook:
                _pullSystem.ActivateHook(partContainer.HookView);
                AppliedPullAbility?.Invoke();
                break;
            default:
                return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        _playerInputs.CharacterControls.Movement.performed -= Move_performed;
        _playerInputs.CharacterControls.Select.performed -= Select_performed;
    }
}