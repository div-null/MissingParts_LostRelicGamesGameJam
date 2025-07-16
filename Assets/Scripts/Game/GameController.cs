using System;
using Game.Level;
using Game.Storage;
using Game.UI;
using Infrastructure.Scope;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;
using CharacterController = Game.Character.CharacterController;

namespace Game
{
    public class GameController
    {
        public event Action<int> NextLevel;

        private readonly LevelLoader _levelLoader;
        private readonly Ceiling _ceiling;
        private readonly GameUI _gameUI;
        private readonly PlayerInputs _playerInputs;
        private readonly AudioManager _audioManager;
        private readonly GameContainer _gameContainer;

        private int _currentLevel;
        private Field _field;
        private CharacterController _character;

        private IObservable<Unit> _firstInput;
        private LifetimeScope _childScope;
        private CompositeDisposable _levelDisposable;

        public GameController(
            LevelLoader levelLoader,
            PlayerInputs playerInputs,
            GameUI gameUI,
            Ceiling ceiling,
            AudioManager audioManager,
            GameContainer gameContainer)
        {
            _gameContainer = gameContainer;
            _playerInputs = playerInputs;
            _gameUI = gameUI;
            _ceiling = ceiling;
            _levelLoader = levelLoader;
            _currentLevel = 1;
            _audioManager = audioManager;
        }

        public void Start()
        {
            _gameUI.RestartClicked += OnRestart;
            _gameUI.ChooseExtraLevel += OnLevelSelected;
            NextLevel += _gameUI.ToNextLevel;

            StartWith(_currentLevel);
        }

        private void LoadLevel()
        {
            NextLevel?.Invoke(_currentLevel);
            Debug.Log($"Loading Level {_currentLevel}");

            GameLevel level;
            if (TrySelectLevel(_currentLevel, out var result))
                level = _levelLoader.LoadLevel(result);
            else
            {
                EndOfTheGame();
                return;
            }

            var resolver = CreateScope();
            var levelFactory = resolver.Resolve<LevelFactory>();

            _field = levelFactory.CreateField(level);
            _character = levelFactory.CreateCharacter(level);

            _character.Finished += OnLevelFinished;
            _character.Died += OnRestart;
        }


        private void StartWith(int level)
        {
            LockInputs();
            _ceiling.MakeTransition(
                () =>
                {
                    _currentLevel = level;
                    LoadLevel();

                    _firstInput = Observable.FromEvent(
                            h => _character.Moved += h,
                            h => _character.Moved -= h)
                        .First();
                    _firstInput.Subscribe(_ => FirstPlayerInput());
                },
                UnlockInputs);
        }

        private void OnRestart()
        {
            LockInputs();
            _ceiling.MakeTransition(
                ReloadLevel,
                UnlockInputs
            );
        }

        private void OnLevelFinished()
        {
            LockInputs();
            _ceiling.MakeTransition(LoadNextLevel, UnlockInputs);
        }

        private void OnLevelSelected(int level)
        {
            LockInputs();
            _ceiling.MakeTransition(
                () =>
                {
                    _currentLevel = level;
                    LoadLevel();
                },
                UnlockInputs);
        }

        private void ReloadLevel()
        {
            Debug.Log("Level reloaded");
            DestroyLevel();
            LoadLevel();
        }


        private void LoadNextLevel()
        {
            Debug.Log("Next level loaded");
            DestroyLevel();
            _currentLevel++;
            LoadLevel();
        }

        private void EndOfTheGame()
        {
            _gameUI.ShowCredits();
        }

        private IObjectResolver CreateScope()
        {
            _childScope = _gameContainer.CreateChild(new LevelInstaller());
            return _childScope.Container;
        }

        private void FirstPlayerInput()
        {
            _gameUI.HideMenu();
            _playerInputs.CharacterControls.Restart.performed += _ => OnRestart();
        }

        private void DestroyLevel()
        {
            Debug.Log("Level destroyed");
            _character.Finished -= OnLevelFinished;
            _character.Died -= OnRestart;
            Object.Destroy(_childScope);
            _levelDisposable?.Dispose();
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


        private static bool TrySelectLevel(int level, out LevelLoader.Level result)
        {
            result = level switch
            {
                1 => LevelLoader.Level.Lvl1,
                2 => LevelLoader.Level.Lvl2,
                3 => LevelLoader.Level.Lvl3,
                4 => LevelLoader.Level.Lvl4,
                5 => LevelLoader.Level.Lvl5,
                6 => LevelLoader.Level.Lvl6,
                7 => LevelLoader.Level.Lvl7,
                8 => LevelLoader.Level.Lvl8,
                9 => LevelLoader.Level.Lvl9,
                10 => LevelLoader.Level.Lvl10,
                11 => LevelLoader.Level.Lvl11,
                12 => LevelLoader.Level.Lvl12,
                13 => LevelLoader.Level.Lvl13,
                14 => LevelLoader.Level.Lvl14,
                15 => LevelLoader.Level.Lvl15,
                16 => LevelLoader.Level.Lvl16,
                17 => LevelLoader.Level.Lvl17,
                // 18 => LevelLoader.Level.Lvl18,
                21 => LevelLoader.Level.Lvl21,
                _ => LevelLoader.Level.Unknown
            };

            return result != LevelLoader.Level.Unknown;
        }
    }
}