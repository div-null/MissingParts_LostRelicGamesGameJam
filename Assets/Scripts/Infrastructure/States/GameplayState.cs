using Infrastructure.Scope;
using UnityEngine;
using VContainer.Unity;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private SceneLoader _loader;
        private GameSettings _settings;
        private LifetimeScope _gameScope;

        public GameplayState(SceneLoader loader, GameSettings settings)
        {
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
        }

        public void Exit()
        {
            _gameScope.Dispose();
            Debug.Log("Exited Gameplay state");
        }
    }
}