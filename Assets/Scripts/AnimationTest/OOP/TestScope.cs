using Core.Tests;
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

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(prefab);
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(positionComposer);
            builder.RegisterInstance<ITestCaseFactory<OopAnimationTestCase>>(defaultParameters);
            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}