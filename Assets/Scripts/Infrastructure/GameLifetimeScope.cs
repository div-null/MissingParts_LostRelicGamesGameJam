using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStartup>();
        }
    }
}
