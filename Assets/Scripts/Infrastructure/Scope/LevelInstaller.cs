using Game;
using Game.Character;
using Game.Systems;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<AttachmentSystem>(Lifetime.Scoped);
            builder.Register<MoveSystem>(Lifetime.Scoped);
            builder.Register<RotationSystem>(Lifetime.Scoped);
            builder.Register<PullSystem>(Lifetime.Scoped);
            builder.Register<PitSystem>(Lifetime.Scoped);
            builder.Register<FinishSystem>(Lifetime.Scoped);

            builder.Register<Field>(Lifetime.Scoped);
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.Register<CharacterFactory>(Lifetime.Scoped);
            builder.Register<CharacterController>(Lifetime.Scoped);

            builder.RegisterBuildCallback(resolver => { resolver.Resolve<PlayerInputs>().Enable(); });
        }
    }
}