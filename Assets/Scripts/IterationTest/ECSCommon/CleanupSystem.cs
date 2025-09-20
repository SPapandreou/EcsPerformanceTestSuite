using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace IterationTest.ECSCommon
{
    [BurstCompile]
    [DisableAutoCreation]
    public partial struct CleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}