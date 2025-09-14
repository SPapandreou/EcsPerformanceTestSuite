using Unity.Entities;
using Unity.Mathematics;

namespace IterationTest.ECS
{
    public struct IterationTestData : IComponentData
    {
        public int Size;
        public float X;
        public float Y;
        public float Z;
        
        public float3 Velocity;
    }   
}