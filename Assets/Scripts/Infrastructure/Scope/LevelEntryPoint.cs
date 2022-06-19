using System;
using Game;
using LevelEditor;
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
            
            RegisterCallback(2, () => Debug.Log("Second level"));
        }

        private void RegisterCallback(int value, Action callback)
        {
            NextLevel += v =>
            {
                if (v == value)
                    callback();
            };
        }

        public void Start()
        {
            _gameUI.RestartClicked += ReloadLevel;
            _gameUI.ChooseExtraLevel += LoadExtraLevel;
            _playerInputs.CharacterControls.Movement.performed += FirstPlayerInput;
            LoadNextLevel();
            _ceiling.FadeOut();
        }

        private void FirstPlayerInput(InputAction.CallbackContext obj)
        {
            _gameUI.HideMenu();
            _playerInputs.CharacterControls.Movement.performed -= FirstPlayerInput;
            _playerInputs.CharacterControls.Restart.performed += ReloadLevel;
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
                case 14:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl14);
                    break;
                case 15:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl15);
                    break;
                case 16:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl16);
                    break;
                case 17:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl17);
                    break;
                //case 18:
                //    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl18);
                //    break;
                case 21:
                    level = _levelLoader.LoadLevel(LevelLoader.Level.Lvl21);
                    break;
                default:
                    EndOfTheGame();
                    return;
            }

            _field = _factory.CreateField(level);
            _character = _factory.CreateCharacter(level, _field);
            _character.Moved += _field.CheckForFinish;
            _character.AppliedPullAbility += _field.CheckForFinish;
            _character.AppliedRotateAbility += _field.CheckForFinish;
            _field.Finished += LevelFinished;
            _character.Died += ReloadLevel;
            NextLevel += _gameUI.ToNextLevel;
            _character.Moved += _audioManager.PlayMove;
            //_character.Detached += _audioManager.PlayDetach;
            //_character.AppliedPullAbility += _audioManager.PlayPullIn;
            //_character.AppliedRotateAbility += _audioManager.PlayRotate;
            //TODO: event to reload level died and click on reload button
            //TODO: event to pass load next level after winning on current level
        }


        public void LoadNextLevel()
        {
            _currentLevel++;
            NextLevel?.Invoke(_currentLevel);
            LoadLevel();
        }

        public void ReloadLevel()
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnReloadLevelTransition;
        }
        
        public void ReloadLevel(InputAction.CallbackContext obj)
        {
            LockInputs();
            _ceiling.FadeIn();
            _ceiling.OnFadeIn += OnReloadLevelTransition;
        }
        
        public void LoadExtraLevel(int level)
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
            _character.Moved -= _field.CheckForFinish;
            _character.AppliedPullAbility -= _field.CheckForFinish;
            _character.AppliedRotateAbility -= _field.CheckForFinish;
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