using Unity.Entities;
using Unity.Mathematics;

namespace AnimationTest.ECSCommon
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