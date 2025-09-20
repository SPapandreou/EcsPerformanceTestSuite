using System;
using Core;
using Core.Shapes;

namespace PhysicsTest
{
    public static class ArrangementShapeExtensions
    {
        public static IArrangementGenerator GetGenerator(this ArrangementShape shape, int count, PrimitiveShape primitiveShape, float scale, float offset, float packingFactor)
        {
            return shape switch
            {
                ArrangementShape.Plane => new PlaneArrangement(count, primitiveShape, scale, offset, packingFactor),
                ArrangementShape.Wall => new WallArrangement(count, primitiveShape, scale, offset, packingFactor),
                ArrangementShape.Sphere => new SphereArrangement(count, primitiveShape, scale, offset, packingFactor),
                ArrangementShape.Cube => new CubeArrangement(count, primitiveShape, scale, offset, packingFactor),
                ArrangementShape.Pyramid => new PyramidArrangement(count, primitiveShape, scale, offset, packingFactor),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }
    }
}