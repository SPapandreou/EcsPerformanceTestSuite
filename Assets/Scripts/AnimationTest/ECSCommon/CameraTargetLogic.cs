using AnimationTest.ECS;
using Core.EcsWorld;
using Latios.Transforms;
using Unity.Entities;
using UnityEngine;
using VContainer.Unity;

namespace AnimationTest.ECSCommon
{
    public class CameraTargetLogic : IStartable, ITickable
    {
        private EntityQuery _entityQuery;
        private readonly Transform _cameraTarget;
        private readonly WorldContainer _worldContainer;

        public CameraTargetLogic(WorldContainer worldContainer, Transform cameraTarget)
        {
            _cameraTarget = cameraTarget;
            _worldContainer = worldContainer;
        }

        public void Start()
        {
            _entityQuery = _worldContainer.World.EntityManager.CreateEntityQuery(typeof(CenterPeterTag));
        }

        public void Tick()
        {
            if (_entityQuery.CalculateEntityCount() == 0) return;
            
            var entity = _entityQuery.GetSingletonEntity();
            var transform = _worldContainer.World.EntityManager.GetComponentData<WorldTransform>(entity);
            
            _cameraTarget.position = transform.position;
        }
    }
}