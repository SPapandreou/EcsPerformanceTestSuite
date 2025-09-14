using Unity.Entities;
using Unity.Mathematics;

namespace AnimationTest.ECS
{
    public struct AnimationTestStateData : IComponentData
    {
        public PeterState State;

        public float Remaining;
        public float Progress;
        public float ClipTime;
        public float TransitionTime;
        public PeterState LastState;
        public float LastClipTime;
        public quaternion StartRotation;
        public quaternion TargetRotation;
    }
}