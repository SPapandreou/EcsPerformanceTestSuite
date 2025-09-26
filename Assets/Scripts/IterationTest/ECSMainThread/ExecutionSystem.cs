using IterationTest.ECSCommon;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace IterationTest.ECSMainThread
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

            var velocity = data.Velocity;

            var job = new UpdatePositionJob
            {
                Velocity = velocity
            };
            
            job.Run();
        }
        
        public partial struct UpdatePositionJob : IJobEntity
        {
            [ReadOnly] public float3 Velocity;

            public void Execute(ref LocalTransform transform)
            {
                transform.Position += Velocity;
            }
        }
    }
}