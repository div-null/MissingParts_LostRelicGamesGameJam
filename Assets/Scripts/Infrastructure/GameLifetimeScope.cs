using Infrastructure.States;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope, ICoroutineRunner
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(this).As<ICoroutineRunner>();
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStartup>();
        }
    }
}
