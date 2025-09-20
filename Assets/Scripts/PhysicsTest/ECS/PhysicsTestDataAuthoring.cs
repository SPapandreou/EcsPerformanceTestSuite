using Unity.Entities;
using UnityEngine;

namespace PhysicsTest.ECS
{
    public class PhysicsTestDataAuthoring : MonoBehaviour
    {
        public GameObject cube;
        public GameObject sphere;
        public GameObject capsule;

        public class PhysicsTestDataBaker : Baker<PhysicsTestDataAuthoring>
        {
            public override void Bake(PhysicsTestDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var physicsTestData = new PhysicsTestData();

                physicsTestData.Cube = GetEntity(authoring.cube,
                    TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                physicsTestData.Sphere = GetEntity(authoring.sphere,
                    TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);
                physicsTestData.Capsule = GetEntity(authoring.capsule,
                    TransformUsageFlags.Dynamic | TransformUsageFlags.Renderable);

                AddComponent(entity, physicsTestData);
            }
        }
    }
}