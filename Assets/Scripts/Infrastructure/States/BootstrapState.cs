namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
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