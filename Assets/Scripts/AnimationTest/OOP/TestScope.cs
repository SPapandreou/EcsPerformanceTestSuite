using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AnimationTest.OOP
{
    public class TestScope : LifetimeScope
    {
        public PeterController prefab;
        public Camera mainCamera;
        public CinemachinePositionComposer  positionComposer;
        public DefaultParameters defaultParameters;
        public TestHudView testHudView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(prefab);
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(positionComposer);
            builder.RegisterComponent(testHudView);
            builder.RegisterInstance<ITestCaseFactory<OopAnimation>>(defaultParameters);

            builder.Register<FpsCounter>(Lifetime.Singleton);
            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Transient);
            
            builder.RegisterEntryPoint<TestLogic>();
            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();
        }
    }
}