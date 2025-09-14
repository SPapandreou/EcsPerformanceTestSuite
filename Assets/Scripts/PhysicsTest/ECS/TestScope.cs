using Core;
using VContainer;
using VContainer.Unity;

namespace PhysicsTest.ECS
{
    public class TestScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var worldContainer = new WorldContainer("PhysicsTest");
            worldContainer.InstallUnitySystems();

            builder.Register(_ => worldContainer, Lifetime.Singleton);
            
        }
    }
}