using Core.Shapes;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PhysicsTest.OOP
{
    public class TestScope : LifetimeScope
    {
        public GameObject sphere;
        public GameObject cube;
        public GameObject capsule;
        public TestHudView testHudView;
        public DefaultParameters defaultParameters;
        public Camera mainCamera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var primitives = new GameObject[4];
            primitives[(int)PrimitiveShape.Sphere] = sphere;
            primitives[(int)PrimitiveShape.Cube] = cube;
            primitives[(int)PrimitiveShape.Capsule] = capsule;

            builder.RegisterInstance<ITestCaseFactory<OopPhysics>>(defaultParameters);
            builder.RegisterComponent(mainCamera);
            
            builder.RegisterInstance(primitives);
            builder.RegisterComponent(testHudView);

            builder.Register<FpsCounter>(Lifetime.Singleton);
            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();

            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}