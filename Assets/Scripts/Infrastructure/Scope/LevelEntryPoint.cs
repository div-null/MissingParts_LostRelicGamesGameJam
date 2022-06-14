using Game;
using LevelEditor;
using UnityEngine;
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
        

        public LevelEntryPoint(LevelFactory factory, LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            _factory = factory;
            _currentLevel = 0;
        }

        public void Start()
        {
            LoadNextLevel();
        }

        public void LoadLevel()
        {
            Debug.Log($"Loading Level {_currentLevel}");
            GameLevel level = _currentLevel switch
            {
                1 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl1),
                2 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl2),
                3 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl3),
            };
            _field = _factory.CreateField(level);
            _character = _factory.CreateCharacter(level, _field);
            _character.Moved += _field.CheckForFinish;
            _field.Finished += LevelFinished;
            _character.Died += ReloadLevel;
            //TODO: event to reload level died and click on reload button
            //TODO: event to pass load next level after winning on current level
        }

        private void LevelFinished()
        {
            DestroyLevel();
            LoadNextLevel();
        }

        public void LoadNextLevel()
        {
            _currentLevel++;
            LoadLevel();
        }

        public void ReloadLevel()
        {
            DestroyLevel();
            LoadLevel();
        }

        public void DestroyLevel()
        {
            _character.Moved -= _field.CheckForFinish;
            _field.Finished -= LevelFinished;
            _field.DestroyField();
            _character.DestroyCharacter();
        }
    }
}