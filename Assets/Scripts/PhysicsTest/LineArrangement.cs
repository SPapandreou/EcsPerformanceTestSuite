using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class LineArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new();
        private readonly List<Quaternion> _rotations = new();
        
        public LineArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale,  packingFactor);

            _positions.Add(new Vector3(0, offset, 0));
            _rotations.Add(Quaternion.identity);
            
            for (int i = 1; i < count; i++)
            {
                _positions.Add(new Vector3(spacing * i, offset, 0));
                _rotations.Add(Quaternion.Euler(i * 47 % 360, i * 31 % 360, i * 13 % 360));
            }

            var offsetVector = new Vector3(-count * spacing / 2f, 0, 0);
            _positions = _positions.Select(p => p + offsetVector).ToList();
            
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