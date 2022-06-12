using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private SceneLoader _loader;

        public BootstrapState(SceneLoader loader)
        {
            _loader = loader;
        }
        
        public void Exit()
        {
            Debug.Log("Exited Boostrap state");
            throw new System.NotImplementedException();
        }

        public void Enter()
        {
            Debug.Log("Entered Boostrap state");
            throw new System.NotImplementedException();
        }
    }
}