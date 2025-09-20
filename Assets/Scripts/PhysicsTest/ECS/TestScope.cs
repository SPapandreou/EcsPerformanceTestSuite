using Core.EcsWorld;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PhysicsTest.ECS
{
    public class TestScope : LifetimeScope
    {
        public TestHudView testHudView;
        public DefaultParameters defaultParameters;
        public Camera mainCamera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var worldContainer = new WorldContainer("PhysicsTest");
            worldContainer.InstallUnitySystems();
            worldContainer.UpdateWorld();

            builder.Register(_ => worldContainer, Lifetime.Singleton);

            builder.RegisterInstance<ITestCaseFactory<EcsPhysics>>(defaultParameters);
            builder.RegisterComponent(mainCamera);

            builder.RegisterComponent(testHudView);
            
            builder.Register<FpsCounter>(Lifetime.Singleton);
            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Singleton);

            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();
            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}