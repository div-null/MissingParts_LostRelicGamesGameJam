using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<CharacterFactory>(Lifetime.Scoped);
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.RegisterEntryPoint<LevelEntryPoint>();

            builder.Register(container =>
            {
                var levelEntryPoint = container.Resolve<LevelFactory>();
                return levelEntryPoint.Create(5, 5);
            }, Lifetime.Scoped);

            // builder.Register(container =>
            // {
            //     var characterFactory = container.Resolve<CharacterFactory>();
            //     return characterFactory.Create();
            // }, Lifetime.Scoped);
            
            // builder.RegisterEntryPointExceptionHandler(exception =>
            // {
            //     Debug.Log(exception);
            //     Debug.Log(exception.StackTrace);
            // });
        }
    }
}