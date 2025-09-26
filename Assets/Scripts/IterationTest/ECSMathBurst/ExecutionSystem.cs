using IterationTest.ECSCommon;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace IterationTest.ECSMathBurst
{
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct ExecutionSystem : ISystem
    {
        private const float Freq = 0.434f;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<IterationTestData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new MatrixMultJob().ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }
        
        [BurstCompile]
        public partial struct MatrixMultJob : IJobEntity
        {
            public void Execute(ref LocalTransform transform)
            {
                var p = transform.Position;
                var v = new float4(p, 1f);

                for (int i = 0; i < 100; i++)
                {
                    v = math.sin(v) + math.sqrt(v * 1.2345f);
                    v = math.log(v + 1.001f) * math.exp(v * 0.1f);
                }

                transform.Position = v.xyz;
            }
        }
    }
}