using Core;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AnimationTest.ECS
{
    public class TestScope : LifetimeScope
    {
        public Camera mainCamera;
        public CinemachinePositionComposer  positionComposer;
        public Transform cameraTarget;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(mainCamera);
            builder.RegisterComponent(positionComposer);
            builder.RegisterComponent(cameraTarget);

            var worldContainer = new WorldContainer("EcsAnimationTestCase");
            
            worldContainer.InstallUnitySystems();
            worldContainer.InstallKinemation();
            worldContainer.UpdateWorld();

            builder.Register(_ => worldContainer, Lifetime.Singleton);

            builder.RegisterEntryPoint<CameraTargetLogic>();
            builder.RegisterEntryPoint<TestLogic>();
        }

    }
}