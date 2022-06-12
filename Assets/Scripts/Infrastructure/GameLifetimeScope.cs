using Infrastructure.States;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope, ICoroutineRunner
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterStateMachine(builder);
            builder.RegisterEntryPoint<GameStartup>();
        }

        private void RegisterStateMachine(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        }
    }
}