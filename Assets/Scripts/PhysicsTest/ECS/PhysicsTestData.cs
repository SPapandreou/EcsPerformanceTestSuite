using Unity.Entities;

namespace PhysicsTest.ECS
{
    public struct PhysicsTestData : IComponentData
    {
        public Entity Cube;
        public Entity Sphere;
        public Entity Capsule;
    }
}