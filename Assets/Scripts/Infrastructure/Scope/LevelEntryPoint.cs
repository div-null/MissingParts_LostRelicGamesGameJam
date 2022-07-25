using System;
using Game;
using LevelEditor;
using Systems;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
    {
        //public event Action FinishGame;
        public event Action<int> NextLevel;

        private readonly LevelFactory _factory;
        private readonly LevelLoader _levelLoader;
        private readonly Ceiling _ceiling;
        private readonly GameUI _gameUI;
        private readonly PlayerInputs _playerInputs;
        private int _currentLevel;
        private Field _field;
        private Character _character;
        private AudioManager _audioManager;

        private FinishSystem _finishSystem;
        private CompositeDisposable _disposable;
        private IObservable<Unit> _firstInput;


        public LevelEntryPoint(LevelFactory factory,
            LevelLoader levelLoader,
            PlayerInputs playerInputs,
            GameUI gameUI,
            Ceiling ceiling,
            AudioManager audioManager,
            FinishSystem finishSystem)
        {
            _playerInputs = playerInputs;
            _gameUI = gameUI;
            _ceiling = ceiling;
            _levelLoader = levelLoader;
            _factory = factory;
            _currentLevel = 0;
            _ceiling.OnFadeOut += UnlockInputs;
            _audioManager = audioManager;
            _finishSystem = finishSystem;
        }

        public void Start()
        {
            _firstInput = Observable.FromEvent(h => _character.Moved += h, h => _character.Moved -= h).Single();
            _firstInput.Subscribe(_ => FirstPlayerInput());

            _gameUI.RestartClicked += ReloadLevel;
            _gameUI.ChooseExtraLevel += LoadExtraLevel;
            LoadNextLevel();
            _ceiling.FadeOut();
        }

        private void FirstPlayerInput()
        {
            _gameUI.HideMenu();
            _character.Moved -= FirstPlayerInput;
            _playerInputs.CharacterControls.Restart.performed += ReloadLevel;
        }

        private void LoadLevel()
        {
            GameLevel? level = SelectNextLevel();

            if (level == null)
            {
                EndOfTheGame();
                return;
            }

            _field = _factory.CreateField(level);
            _character = _factory.CreateCharacter(level, _field);

            IObservable<Unit> characterUpdated = Observable.Concat(
                Observable.FromEvent(h => _character.Moved += h, h => _character.Moved -= h),
                Observable.FromEvent(h => _character.AppliedPullAbility += h, h => _character.AppliedPullAbility -= h),
                Observable.FromEvent(h => _character.AppliedRotateAbility += h, h => _character.AppliedRotateAbility -= h)
            );

            _disposable = new CompositeDisposable();

            characterUpdated.Subscribe(_ => _finishSystem.CheckForFinish()).AddTo(_disposable);
            _finishSystem.Finished += LevelFinished;
            _character.Died += ReloadLevel;
            NextLevel += _gameUI.ToNextLevel;

            _character.Moved += _audioManager.PlayMove;
            //_character.Detached += _audioManager.PlayDetach;
            //_character.AppliedPullAbility += _audioManager.PlayPullIn;
            //_character.AppliedRotateAbility += _audioManager.PlayRotate;
            //TODO: event to reload level died and click on reload button
            //TODO: event to pass load next level after winning on current level
        }


        private void LoadNextLevel()
        {
            _currentLevel++;
            NextLevel?.Invoke(_currentLevel);
            LoadLevel();
        }

        private void ReloadLevel()
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnReloadLevelTransition;
        }

        private void ReloadLevel(InputAction.CallbackContext obj)
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnReloadLevelTransition;
        }

        private void LoadExtraLevel(int level)
        {
            //fix fade in
            LockInputs();
            _currentLevel = level;
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnExtraLevelTransition;
        }

        private void LevelFinished()
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnNextLevelTransition;
        }

        private void OnNextLevelTransition()
        {
            _ceiling.OnFadeIn -= OnNextLevelTransition;
            DestroyLevel();
            LoadNextLevel();
            _ceiling.FadeOut();
        }

        private void OnReloadLevelTransition()
        {
            _ceiling.OnFadeIn -= OnReloadLevelTransition;
            DestroyLevel();
            LoadLevel();
            _ceiling.FadeOut();
        }

        private void OnExtraLevelTransition()
        {
            _ceiling.OnFadeIn -= OnExtraLevelTransition;
            LoadLevel();
            _ceiling.FadeOut();
        }

        private void DestroyLevel()
        {
            _finishSystem.Finished -= LevelFinished;
            _disposable.Dispose();

            _factory.CleanUp();
            _character.Destroy();
        }


        private void LockInputs()
        {
            Debug.Log("Inputs locked");
            _playerInputs.Disable();
        }

        private void UnlockInputs()
        {
            Debug.Log("Inputs unlocked");
            _playerInputs.Enable();
        }

        private void EndOfTheGame()
        {
            _gameUI.ShowCredits();
        }

        private GameLevel? SelectNextLevel()
        {
            Debug.Log($"Loading Level {_currentLevel}");
            return _currentLevel switch
            {
                1 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl1),
                2 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl2),
                3 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl3),
                4 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl4),
                5 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl5),
                6 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl6),
                7 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl7),
                8 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl8),
                9 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl9),
                10 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl10),
                11 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl11),
                12 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl12),
                13 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl13),
                14 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl14),
                15 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl15),
                16 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl16),
                17 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl17),
                // 18 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl18);
                21 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl21),
                _ => null
            };
        }
    }
}