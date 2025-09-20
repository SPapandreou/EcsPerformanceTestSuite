using IterationTest.ECSCommon;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace IterationTest.ECSMathParallel
{
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

            const float time = 1.26788f;

            state.Dependency = new TrigonometryNoiseJob
            {
                Time = time
            }.ScheduleParallel(state.Dependency);

            state.Enabled = false;
        }
        
        public partial struct TrigonometryNoiseJob : IJobEntity
        {
            [ReadOnly]public float Time;
            public void Execute(ref LocalTransform transform)
            {
                float3 p = transform.Position;

                p.x += math.sin(p.y * 0.1f + Time) * 0.5f;
                p.y += math.cos(p.x * 0.1f + Time) * 0.5f;
                p.z += math.sin(p.x * 0.05f + p.y * 0.05f + Time) * 0.25f;
                
                transform.Position =p;
            }
        }
    }
}