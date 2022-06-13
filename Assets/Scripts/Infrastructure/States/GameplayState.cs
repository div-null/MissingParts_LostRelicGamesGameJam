using Infrastructure.Scope;
using LevelEditor;
using UnityEngine;
using VContainer.Unity;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private SceneLoader _loader;
        private GameSettings _settings;
        private LifetimeScope _gameScope;
        private LevelLoader _levelLoader;

        public GameplayState(SceneLoader loader, GameSettings settings, LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            _settings = settings;
            _loader = loader;
        }

        public void Enter()
        {
            Debug.Log("Entered Gameplay state");
            LifetimeScope scope = LifetimeScope.Find<GameLifetimeScope>();
            // var levelInstaller = new LevelInstaller();
            using (LifetimeScope.EnqueueParent(scope))
            {
                _loader.Load("Game", onLoaded);
            }
        }

        private void onLoaded()
        {
            GameLevel gameLevel = _levelLoader.LoadLevel(LevelLoader.Level.Lvl1);
            Debug.Log(gameLevel);
        }

        public void Exit()
        {
            _gameScope.Dispose();
            Debug.Log("Exited Gameplay state");
        }
    }
}