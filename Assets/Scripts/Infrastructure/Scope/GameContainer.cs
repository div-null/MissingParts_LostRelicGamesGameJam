using Game;
using Game.UI;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class GameContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.Register<PlayerInputs>(Lifetime.Scoped);
            builder.Register<GameController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<LevelEntryPoint>();
            builder.RegisterComponentInHierarchy<GameUI>();
            builder.RegisterComponentInHierarchy<AudioManager>();
            builder.RegisterComponentInHierarchy<Ceiling>();
        }
    }
}