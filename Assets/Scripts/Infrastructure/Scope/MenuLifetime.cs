using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class MenuLifetime : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}
