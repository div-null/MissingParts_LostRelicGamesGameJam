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
        private AudioManager _audioManager;


        public LevelEntryPoint(LevelFactory factory, LevelLoader levelLoader, PlayerInputs playerInputs, GameUI gameUI, Ceiling ceiling, AudioManager audioManager)
        {
            _playerInputs = playerInputs;
            _gameUI = gameUI;
            _ceiling = ceiling;
            _levelLoader = levelLoader;
            _factory = factory;
            _currentLevel = 0;
            _ceiling.OnFadeOut += UnlockInputs;
            _audioManager = audioManager;
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
            GameLevel level;
            switch (_currentLevel)
            {
                case 1:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl1);
                    break;
                case 2:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl2);
                    break;
                case 3:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl3);
                    break;
                case 4:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl4);
                    break;
                case 5:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl5);
                    break;
                case 6:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl6);
                    break;
                case 7:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl7);
                    break;
                case 8:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl8);
                    break;
                case 9:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl9);
                    break;
                case 10:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl10);
                    break;
                case 11:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl11);
                    break;
                case 12:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl12);
                    break;
                case 13:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl13);
                    break;
                default:
                    EndOfTheGame();
                    return;
            }

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

        private void EndOfTheGame()
        {
            _gameUI.ShowCredits();
        }
    }
}