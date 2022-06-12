using UnityEngine;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private SceneLoader _loader;

        public GameplayState(SceneLoader loader)
        {
            _loader = loader;
        }

        public void Enter()
        {
            Debug.Log("Entered Gameplay state");
            _loader.Load("Game");
        }

        public void Exit()
        {
            Debug.Log("Exited Gameplay state");
        }
    }
}