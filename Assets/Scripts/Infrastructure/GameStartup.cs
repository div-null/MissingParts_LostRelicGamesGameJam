using Infrastructure.States;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameStartup : IStartable
    {
        private GameStateMachine _stateMachine;

        public GameStartup(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Start()
        {
            _stateMachine.Enter<BootstrapState>();
        }
    }
}