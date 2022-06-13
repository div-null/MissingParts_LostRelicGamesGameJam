using System.Collections.Generic;
using Infrastructure.States;
using UnityEngine.SceneManagement;
using LevelEditor;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class GameLifetimeScope : LifetimeScope, ICoroutineRunner
    {
        public GameSettings GameSettings;

        protected override void Awake()
        {
   var containers = FindObjectsOfType<GameLifetimeScope>();
            if (SceneManager.GetActiveScene().name == "Initial")
            {
                DontDestroyOnLoad(this);
                foreach (var container in containers)
                {
                    var scope = gameObject.GetComponent<GameLifetimeScope>();
                    if (scope != null && scope != container)
                        Destroy(container.gameObject);
                }
            }
            else
            {
                if (containers.Length > 1)
                {
                    Destroy(this.gameObject);
                    return;
                }
            }

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<GameSettings>(GameSettings);
            RegisterStateMachine(builder);
            builder.RegisterComponentInHierarchy<CoroutineRunner>().AsImplementedInterfaces();
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<LevelLoader>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStartup>();
        }

        private void RegisterStateMachine(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<GameplayState>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<GameStateMachine>(Lifetime.Scoped);
            builder.RegisterBuildCallback(container =>
            {
                var states = container.Resolve<IEnumerable<IExitableState>>();
                var stateMachine = container.Resolve<GameStateMachine>();
                stateMachine.AddStates(states);
            });
        }
    }
}