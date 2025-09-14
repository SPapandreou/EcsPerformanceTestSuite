using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public class BootstrapScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainEntryPoint>();
        }
    }
}