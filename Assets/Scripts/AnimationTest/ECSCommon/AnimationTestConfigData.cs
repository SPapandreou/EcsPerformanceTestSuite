using Latios.Kinemation;
using Unity.Entities;

namespace AnimationTest.ECSCommon
{
    public struct AnimationTestConfigData : IComponentData
    {
        public Entity PeterPrefab;
        public BlobAssetReference<SkeletonClipSetBlob> Clips;
        public float WalkDistance;
        public float WalkSpeed;
        
    }
}