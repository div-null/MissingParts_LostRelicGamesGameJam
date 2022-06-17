using Game;
using LevelEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
    {
        private LevelFactory _factory;
        private int _currentLevel;
        private LevelLoader _levelLoader;

        private Field _field;
        private Character _character;
        private Ceiling _ceiling;
        private GameUI _gameUI;
        private PlayerInputs _playerInputs;


        public LevelEntryPoint(LevelFactory factory, LevelLoader levelLoader, PlayerInputs playerInputs, GameUI gameUI, Ceiling ceiling)
        {
            _playerInputs = playerInputs;
            _gameUI = gameUI;
            _ceiling = ceiling;
            _levelLoader = levelLoader;
            _factory = factory;
            _currentLevel = 1;
            _ceiling.OnFadeOut += UnlockInputs;
        }

        public void Start()
        {
            _gameUI.RestartClicked += ReloadLevel;
            _playerInputs.CharacterControls.Movement.performed += FirstPlayerInput;
            LoadNextLevel();
            _ceiling.FadeOut();
        }

        private void FirstPlayerInput(InputAction.CallbackContext obj)
        {
            _gameUI.HideMenu();
            _playerInputs.CharacterControls.Movement.performed -= FirstPlayerInput;
        }

        public void LoadLevel()
        {
            Debug.Log($"Loading Level {_currentLevel}");
            GameLevel level = _currentLevel switch
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
            };
            _field = _factory.CreateField(level);
            _character = _factory.CreateCharacter(level, _field);
            _character.Moved += _field.CheckForFinish;
            _field.Finished += LevelFinished;
            _character.Died += ReloadLevel;
            //TODO: event to reload level died and click on reload button
            //TODO: event to pass load next level after winning on current level
        }

        public void LoadNextLevel()
        {
            _currentLevel++;
            LoadLevel();
        }

        public void ReloadLevel()
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnReloadLevelTransition;
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

        private void DestroyLevel()
        {
            _character.Moved -= _field.CheckForFinish;
            _field.Finished -= LevelFinished;
            _field.DestroyField();
            _character.DestroyCharacter();
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
    }
}