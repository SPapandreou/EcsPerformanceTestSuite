using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class CubeArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new();
        private readonly List<Quaternion> _rotations = new();

        public CubeArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale, packingFactor);

            var size = Mathf.CeilToInt(Mathf.Pow(count, 1f / 3f));
            var generated = 0;
            for (var x = 0; x < size && generated < count; x++)
            {
                for (var y = 0; y < size && generated < count; y++)
                {
                    for (var z = 0; z < size && generated < count; z++)
                    {
                        var position = new Vector3(x * spacing, y * spacing, z * spacing);
                        _positions.Add(position);
                        _rotations.Add(Quaternion.Euler(generated * 47 % 360, generated * 31 % 360,
                            generated * 13 % 360));
                        generated++;
                    }
                }
            }

            var offsetVector = new Vector3(-((size - 1) * spacing) / 2, -((size - 1) * spacing) / 2 + offset,
                -((size - 1) * spacing) / 2);
            
            _positions = _positions.Select(p=>p+offsetVector).ToList();
        }

        public List<Vector3> GetVectors()
        {
            return _positions;
        }

        public List<float3> GetFloats()
        {
            return _positions.Select(p=>(float3)p).ToList();
        }

        public List<Quaternion> GetRotations()
        {
            return _rotations;
        }
    }
}