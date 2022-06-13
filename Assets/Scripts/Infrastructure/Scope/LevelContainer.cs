using Game;
using LevelEditor;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.RegisterEntryPoint<LevelEntryPoint>();
        }
    }
}