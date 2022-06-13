using LevelEditor;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private SceneLoader _loader;
        private GameStateMachine _stateMachine;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader loader, LevelLoader levelLoader)
        {
            _stateMachine = stateMachine;
            _levelLoader = levelLoader;
            _loader = loader;
        }

        public void Exit()
        {
            Debug.Log("Exited Boostrap state");
        }

        public void Enter()
        {
            Debug.Log("Entered Boostrap state");
            _loader.Load("Initial", onLoaded);
        }

        private void onLoaded()
        {
            _stateMachine.Enter<GameplayState>();
        }
    }
}