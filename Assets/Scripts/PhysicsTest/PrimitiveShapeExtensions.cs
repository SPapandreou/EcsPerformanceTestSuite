using System;
using Core;
using UnityEngine;

namespace PhysicsTest
{
    public static class PrimitiveShapeExtensions
    {
        public static float GetSpacing(this PrimitiveShape shape, float scale, float packingFactor)
        {
            switch (shape)
            {
                case PrimitiveShape.Sphere:
                    return scale * packingFactor;

                case PrimitiveShape.Cube:
                    var diagonal = Mathf.Sqrt(3f) * scale;
                    return diagonal * packingFactor;
                
                case PrimitiveShape.Capsule:
                    var height = 2f * scale;
                    var radius = 0.5f * scale;
                    float containingSphereRadius = Mathf.Sqrt((height * 0.5f) * (height * 0.5f) + radius * radius);
                    return 2f * containingSphereRadius * packingFactor;

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }
    }
}