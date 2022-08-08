using Game;
using Game.Systems;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<AttachmentSystem>(Lifetime.Transient);
            builder.Register<FinishSystem>(Lifetime.Transient);
            builder.Register<MoveSystem>(Lifetime.Transient);
            builder.Register<RotationSystem>(Lifetime.Transient);
            builder.Register<PullSystem>(Lifetime.Transient);
            builder.Register<PitSystem>(Lifetime.Transient);

            builder.Register<Field>(Lifetime.Scoped);
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.RegisterBuildCallback(resolver => { resolver.Resolve<PlayerInputs>().Enable(); });
        }
    }
}