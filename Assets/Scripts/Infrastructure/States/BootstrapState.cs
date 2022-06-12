using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private ICoroutineRunner _runner;

        public BootstrapState(ICoroutineRunner runner)
        {
            _runner = runner;
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