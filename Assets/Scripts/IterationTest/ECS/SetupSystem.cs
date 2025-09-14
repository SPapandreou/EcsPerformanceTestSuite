using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace IterationTest.ECS
{
    [BurstCompile]
    [DisableAutoCreation]
    public partial struct SetupSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<IterationTestData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var data = SystemAPI.GetSingleton<IterationTestData>();

            var entityManager = state.WorldUnmanaged.EntityManager;

            var currentPosition = float3.zero;

            var offset = new float3(data.X, data.Y, data.Z);
            
            for (int i = 0; i < data.Size; i++)
            {
                var entity = entityManager.CreateEntity();
                entityManager.AddComponent<LocalTransform>(entity);
                entityManager.SetComponentData(entity, LocalTransform.FromPosition(currentPosition));
                currentPosition += offset;
                offset.xyz = offset.zxy;
            }
            
            state.Enabled = false;
        }
    }
}