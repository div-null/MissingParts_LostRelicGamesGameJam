using System;
using Game;
using Game.Character;
using LevelEditor;
using UI;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CharacterController = Game.Character.CharacterController;
using Object = UnityEngine.Object;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
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


        public LevelEntryPoint(
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
            _currentLevel = 0;
            _audioManager = audioManager;
        }

        public void Start()
        {
            _gameUI.RestartClicked += OnRestart;
            _gameUI.ChooseExtraLevel += OnSelectExtraLevel;
            _currentLevel++;
            NextLevel?.Invoke(_currentLevel);
            LoadLevel();
            _firstInput = Observable.FromEvent(h => _character.Moved += h, h => _character.Moved -= h).First();
            _firstInput.Subscribe(_ => FirstPlayerInput());
            _ceiling.FadeOut();
        }

        private void FirstPlayerInput()
        {
            _gameUI.HideMenu();
            _playerInputs.CharacterControls.Restart.performed += _ => OnRestart();
        }

        private void LoadLevel()
        {
            NextLevel?.Invoke(_currentLevel);
            GameLevel? level = SelectNextLevel();

            if (level == null)
            {
                EndOfTheGame();
                return;
            }

            LevelFactory factory = CreateScopedFactory();

            _field = factory.CreateField(level);
            _character = factory.CreateCharacter(level, _field);

            _character.Finished += OnLevelFinished;
            _character.Died += OnRestart;
            NextLevel += _gameUI.ToNextLevel;

            // _character.Moved += _audioManager.PlayMove;
            //_character.Detached += _audioManager.PlayDetach;
            //_character.AppliedPullAbility += _audioManager.PlayPullIn;
            //_character.AppliedRotateAbility += _audioManager.PlayRotate;
            //TODO: event to reload level died and click on reload button
            //TODO: event to pass load next level after winning on current level
        }


        private void OnRestart()
        {
            LockInputs();
            MakeTransition(
                ReloadLevel,
                UnlockInputs
            );
        }

        private void OnLevelFinished()
        {
            LockInputs();
            MakeTransition(LoadNextLevel, UnlockInputs);
        }

        private void OnSelectExtraLevel(int level)
        {
            LockInputs();
            MakeTransition(
                () =>
                {
                    _currentLevel = level;
                    LoadLevel();
                },
                UnlockInputs);
            _ceiling.FadeIn();
        }

        private void ReloadLevel()
        {
            DestroyLevel();
            LoadLevel();
        }


        private void LoadNextLevel()
        {
            DestroyLevel();
            _currentLevel++;
            LoadLevel();
        }

        private void EndOfTheGame()
        {
            _gameUI.ShowCredits();
        }

        private LevelFactory CreateScopedFactory()
        {
            _levelDisposable = new CompositeDisposable();
            _childScope = _gameContainer.CreateChild(new LevelInstaller());
            var factory = _childScope.Container.Resolve<LevelFactory>();
            _levelDisposable.Add(factory);
            return factory;
        }

        private void DestroyLevel()
        {
            _character.Finished -= OnLevelFinished;
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

        private void MakeTransition(Action onFadeIn, Action onFadeOut)
        {
            CompositeDisposable disposable = new CompositeDisposable();

            _ceiling.OnFadeIn.Subscribe(_ =>
            {
                onFadeIn.Invoke();
                _ceiling.FadeOut();
            }).AddTo(disposable);

            _ceiling.OnFadeOut.Subscribe(_ =>
            {
                onFadeOut.Invoke();
                disposable.Dispose();
            }).AddTo(disposable);
            _ceiling.FadeIn();
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