using Unity.Entities;
using UnityEngine;

namespace AnimationTest.ECS
{
    public class PeterTagAuthoring : MonoBehaviour
    {
        public class PeterTagBaker : Baker<PeterTagAuthoring>
        {
            public override void Bake(PeterTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<PeterTag>(entity);
            }
        }
    }
}