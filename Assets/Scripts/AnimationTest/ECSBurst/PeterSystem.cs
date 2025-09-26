using AnimationTest.ECSCommon;
using Latios.Kinemation;
using Latios.Transforms;
using Latios.Transforms.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AnimationTest.ECSBurst
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(TransformSuperSystem))]
    [BurstCompile]
    public partial struct PeterSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AnimationTestConfigData>();
            state.RequireForUpdate<CenterPeterTag>();
            state.RequireForUpdate<AnimationTestStateData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var testStateData = SystemAPI.GetSingleton<AnimationTestStateData>();
            var config = SystemAPI.GetSingleton<AnimationTestConfigData>();
            var centerPeter = SystemAPI.GetSingletonEntity<CenterPeterTag>();
            var transform = SystemAPI.GetAspect<TransformAspect>(centerPeter);
            
            var deltaTime = state.World.Time.DeltaTime;

            if (testStateData.TransitionTime > 0)
            {
                testStateData.TransitionTime += deltaTime;
            }

            if (testStateData.TransitionTime > 0.25f)
            {
                testStateData.TransitionTime = -1f;
            }

            testStateData.ClipTime += deltaTime;
            testStateData.LastClipTime += deltaTime;


            switch (testStateData.State)
            {
                case PeterState.Walking:
                    if (testStateData.Remaining <= 0)
                    {
                        testStateData.State = PeterState.Turning;
                        testStateData.LastState = PeterState.Walking;
                        testStateData.TransitionTime = 0f;
                        testStateData.ClipTime = 0f;
                        testStateData.Progress = 0f;
                        testStateData.StartRotation = transform.worldRotation;
                        testStateData.TargetRotation =
                            math.mul(testStateData.StartRotation,quaternion.Euler(0, math.radians(-90f), 0));
                    }

                    testStateData.Remaining -= config.WalkSpeed * deltaTime;
                    break;
                case PeterState.Saluting:
                    if (testStateData.Progress >= 1f)
                    {
                        testStateData.State = PeterState.Walking;
                        testStateData.LastState = PeterState.Saluting;
                        testStateData.TransitionTime = 0f;
                        testStateData.ClipTime = 0f;
                        testStateData.Remaining = config.WalkDistance;
                    }
                    
                    var saluteLength = config.Clips.Value.clips[(int)PeterState.Saluting].duration;
                    testStateData.Progress = testStateData.ClipTime / saluteLength;
                    break;
                case PeterState.Turning:
                    if (testStateData.Progress >= 1f)
                    {
                        testStateData.State = PeterState.Saluting;
                        testStateData.LastState = PeterState.Turning;
                        testStateData.TransitionTime = 0f;
                        testStateData.ClipTime = 0f;
                        testStateData.Progress = 0f;
                    }
                    var turnLength = config.Clips.Value.clips[(int)PeterState.Turning].duration;
                    testStateData.Progress = testStateData.ClipTime / turnLength;
                    break;
            }

            SystemAPI.SetSingleton(testStateData);

            state.Dependency.Complete();
            
            state.Dependency = new SampleAnimationJob
            {
                State = testStateData,
                Config = config
            }.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
            
            state.Dependency = new UpdatePeterPositionsJob
            {
                State = testStateData,
                Config = config,
                DeltaTime = deltaTime
            }.ScheduleParallel(state.Dependency);
            
        }

        [BurstCompile]
        public partial struct SampleAnimationJob : IJobEntity
        {
            [ReadOnly] public AnimationTestStateData State;
            [ReadOnly] public AnimationTestConfigData Config;

            public void Execute(OptimizedSkeletonAspect skeleton)
            {
                ref var clip = ref Config.Clips.Value.clips[(int)State.State];
                var clipTime = clip.LoopToClipTime(State.ClipTime);
                var blendWeight = State.TransitionTime > 0 ? 0.5f : 1f;

                clip.SamplePose(ref skeleton, clipTime, blendWeight);
                if (blendWeight < 1f)
                {
                    ref var lastClip = ref Config.Clips.Value.clips[(int)State.LastState];
                    lastClip.SamplePose(ref skeleton, clipTime, blendWeight);
                }

                skeleton.EndSamplingAndSync();
            }
        }
        
        [BurstCompile]
        public partial struct UpdatePeterPositionsJob : IJobEntity
        {
            [ReadOnly] public AnimationTestStateData State;
            [ReadOnly] public AnimationTestConfigData Config;
            [ReadOnly] public float DeltaTime;

            public void Execute(TransformAspect transform)
            {
                if (State.State == PeterState.Walking)
                {
                    transform.TranslateWorld(transform.forwardDirection * (Config.WalkSpeed * DeltaTime));
                    
                }

                if (State.State == PeterState.Turning)
                {
                    var localTransform = transform.localTransformQvvs;
                    localTransform.rotation = math.slerp(State.StartRotation, State.TargetRotation, State.Progress);
                    transform.localTransformQvvs = localTransform;
                }
            }
        }
    }
}