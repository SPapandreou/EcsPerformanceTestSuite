using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public interface IArrangementGenerator
    {
        List<Vector3> GetVectors();
        List<float3> GetFloats();

        List<Quaternion> GetRotations();
    }
}