using Unity.Mathematics;
using UnityEngine;

namespace IterationTest
{
    public static class IterationTestConfiguration
    {
        public const float X = 0f;
        public const float Y = 0.5f;
        public const float Z = 1f;
        
        public const float VelocityX = 0.1f;
        public const float VelocityY = 0.005f;
        public const float VelocityZ = 0.345f;
        
        public static Vector3 VelocityVector => new Vector3(VelocityX, VelocityY, VelocityZ);

        public static float3 VelocityFloat => new float3(VelocityX, VelocityY, VelocityZ);
    }
}