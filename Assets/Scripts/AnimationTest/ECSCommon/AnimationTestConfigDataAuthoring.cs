using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace AnimationTest.ECSCommon
{
    public class AnimationTestConfigDataAuthoring : MonoBehaviour
    {
        public GameObject peterPrefab;

        public AnimationClip walk;
        public AnimationClip turn;
        public AnimationClip salute;

        public float walkDistance;
        public float walkSpeed;

        [TemporaryBakingType]
        public struct ConfigDataSmartBakeItem : ISmartBakeItem<AnimationTestConfigDataAuthoring>
        {
            private SmartBlobberHandle<SkeletonClipSetBlob> _clipBlob;

            public bool Bake(AnimationTestConfigDataAuthoring authoring, IBaker baker)
            {
                var entity = baker.GetEntity(TransformUsageFlags.None);
                baker.AddComponent<AnimationTestConfigData>(entity);
                baker.SetComponent(entity, new AnimationTestConfigData
                {
                    PeterPrefab = baker.GetEntity(authoring.peterPrefab, TransformUsageFlags.None),
                    WalkSpeed = authoring.walkSpeed,
                    WalkDistance = authoring.walkDistance
                });


                var clips = new NativeArray<SkeletonClipConfig>(3, Allocator.Temp);

                clips[(int)PeterState.Walking] = new SkeletonClipConfig
                {
                    clip = authoring.walk,
                    settings = SkeletonClipCompressionSettings.kDefaultSettings
                };

                clips[(int)PeterState.Saluting] = new SkeletonClipConfig
                {
                    clip = authoring.salute,
                    settings = SkeletonClipCompressionSettings.kDefaultSettings
                };

                clips[(int)PeterState.Turning] = new SkeletonClipConfig
                {
                    clip = authoring.turn,
                    settings = SkeletonClipCompressionSettings.kDefaultSettings
                };

                _clipBlob = baker.RequestCreateBlobAsset(authoring.peterPrefab.GetComponent<Animator>(), clips);

                return true;
            }

            public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
            {
                var data = entityManager.GetComponentData<AnimationTestConfigData>(entity);
                data.Clips = _clipBlob.Resolve(entityManager);
                entityManager.SetComponentData(entity, data);
            }
        }

        public class AnimationTestConfigDataBaker : SmartBaker<AnimationTestConfigDataAuthoring, ConfigDataSmartBakeItem>
        {
        }
    }
}