using System;
using Game.Systems;
using LevelEditor;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Character
{
    public class CharacterController : IDisposable
    {
        public event Action Moved;
        public event Action AppliedPullAbility;
        public event Action AppliedRotateAbility;
        public event Action StartMoving;
        private event Action PartDestroyed;
        public event Action Died;
        public event Action Finished;

        private CharacterPart _mainPart;
        private PlayerInputs _playerInputs;
        private PullSystem _pullSystem;
        private RotationSystem _rotationSystem;
        private AttachmentSystem _attachmentSystem;
        private MoveSystem _moveSystem;
        private PitSystem _pitSystem;
        private FinishSystem _finishSystem;
        private IObservable<Unit> _characterUpdated;
        private readonly CompositeDisposable _disposable;
        private int _pullRange;

        public CharacterController(PlayerInputs playerInputs,
            MoveSystem moveSystem,
            RotationSystem rotationSystem,
            AttachmentSystem attachmentSystem,
            PullSystem pullSystem,
            PitSystem pitSystem,
            FinishSystem finishSystem)
        {
            _playerInputs = playerInputs;
            _pitSystem = pitSystem;
            _finishSystem = finishSystem;
            _moveSystem = moveSystem;
            _attachmentSystem = attachmentSystem;
            _pullSystem = pullSystem;
            _rotationSystem = rotationSystem;

            _playerInputs.CharacterControls.Movement.performed += Move_performed;
            _playerInputs.CharacterControls.Select.performed += Select_performed;

            _characterUpdated = Observable.Merge(
                Observable.FromEvent(h => Moved += h, h => Moved -= h),
                Observable.FromEvent(h => AppliedPullAbility += h, h => AppliedPullAbility -= h),
                Observable.FromEvent(h => AppliedRotateAbility += h, h => AppliedRotateAbility -= h),
                Observable.FromEvent(h => PartDestroyed += h, h => PartDestroyed -= h)
            );

            _disposable = new CompositeDisposable();
            _characterUpdated.Subscribe(_ => CheckFinished()).AddTo(_disposable);

            /*_playerInputs.CharacterControls.PrimaryContact.started += StartTouchPrimary;
        _playerInputs.CharacterControls.PrimaryContact.canceled += EndTouchPrimary;*/
        }

        public void Initialize(CharacterPart mainPart, int pullRange)
        {
            _pullRange = pullRange;
            _mainPart = mainPart;
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

        private void Move(DirectionType direction)
        {
            StartMoving?.Invoke();
            if (!_mainPart.IsActive)
                Debug.LogError("Controlling disabled part");

            if (!_moveSystem.Move(_mainPart, direction))
                return;

            // check for pits
            (CharacterPart newMainPart, bool pitDetected) = _pitSystem.PreserveMaxPart(_mainPart);

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

            if (pitDetected)
                PartDestroyed?.Invoke();

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

                        (CharacterPart newHead, bool pitDetected) = _pitSystem.PreserveConnectedPart(_mainPart, partContainer.Part);
                        _mainPart = newHead;

                        if (pitDetected)
                            PartDestroyed?.Invoke();
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
            _mainPart = partContainer.Part;
            switch (partContainer.Part.Ability)
            {
                case AbilityType.Rotation:
                    if (_rotationSystem.TryToRotate(partContainer.Part))
                        AppliedRotateAbility?.Invoke();
                    return true;

                case AbilityType.Hook:
                    if (_pullSystem.ActivateHook(partContainer.HookView, _pullRange))
                        AppliedPullAbility?.Invoke();

                    return true;

                default:
                    return false;
            }
        }

        private void CheckFinished()
        {
            if (_finishSystem.CheckFinished())
                Finished?.Invoke();
        }

        public void Dispose()
        {
            _playerInputs.CharacterControls.Movement.performed -= Move_performed;
            _playerInputs.CharacterControls.Select.performed -= Select_performed;
            _disposable.Dispose();
        }
    }
}