using Infrastructure.States;
using LevelEditor;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class GameLifetimeScope : LifetimeScope, ICoroutineRunner
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterStateMachine(builder);
            builder.RegisterComponentInHierarchy<CoroutineRunner>().AsImplementedInterfaces();
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<LevelLoader>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStartup>();
        }

        private void RegisterStateMachine(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        }
    }
}