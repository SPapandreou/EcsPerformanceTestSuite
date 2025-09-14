using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace IterationTest.ECS
{
    [BurstCompile]
    [DisableAutoCreation]
    public partial struct ExecutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<IterationTestData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var data = SystemAPI.GetSingleton<IterationTestData>();

            var velocity = data.Velocity;

            state.Dependency = new UpdatePositionJob
            {
                Velocity = velocity
            }.ScheduleParallel(state.Dependency);
            
            // foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>())
            // {
            //     transform.ValueRW.Position += velocity;
            // }

            state.Enabled = false;
        }

        [BurstCompile]
        public partial struct UpdatePositionJob : IJobEntity
        {
            [ReadOnly]public float3 Velocity;
            public void Execute(ref LocalTransform transform)
            {
                transform.Position += Velocity;
            }
        }
    }
}