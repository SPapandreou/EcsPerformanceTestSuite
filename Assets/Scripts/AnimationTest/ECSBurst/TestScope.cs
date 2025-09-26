using AnimationTest.ECSCommon;
using Core.EcsWorld;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AnimationTest.ECSBurst
{
    public class TestScope : LifetimeScope
    {
        public Camera mainCamera;
        public CinemachinePositionComposer  positionComposer;
        public Transform cameraTarget;
        public DefaultParameters defaultParameters;
        public TestHudView testHudView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(positionComposer);
            builder.RegisterComponent(cameraTarget);
            builder.RegisterComponent(testHudView);
            builder.RegisterInstance<ITestCaseFactory<EcsAnimationBurst>>(defaultParameters);
            
            builder.Register<FpsCounter>(Lifetime.Singleton);
            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Transient);

            var worldContainer = new WorldContainer("EcsAnimationTestCase");
            
            worldContainer.InstallUnitySystems();
            worldContainer.InstallKinemation();
            worldContainer.UpdateWorld();

            builder.Register(_ => worldContainer, Lifetime.Singleton);

            builder.RegisterEntryPoint<CameraTargetLogic>();
            builder.RegisterEntryPoint<TestLogic>();
            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();
        }
        
    }
}