using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class WallArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new ();
        private readonly List<Quaternion> _rotations = new ();
        
        public WallArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale, packingFactor);
            
            var columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            var rows = Mathf.CeilToInt((float)count / columns);

            var generated = 0;
            for (var y = 0; y < rows && generated < count; y++)
            {
                for (var x = 0; x < columns && generated < count; x++)
                {
                    var position = new Vector3(x * spacing, y * spacing, 0);
                    _positions.Add(position);
                    _rotations.Add(Quaternion.Euler(generated * 47 % 360, generated * 31 % 360, generated * 13 % 360));
                    generated++;
                }
            }

            var width = (columns - 1) * spacing;

            var offsetVector = new Vector3(-width / 2, offset, 0);

            _positions = _positions.Select(p => p + offsetVector).ToList();
        }

        public List<Vector3> GetVectors()
        {
            return _positions;
        }

        public List<float3> GetFloats()
        {
            return _positions.Select(p => (float3)p).ToList();
        }

        public List<Quaternion> GetRotations()
        {
            return _rotations;
        }
    }
}