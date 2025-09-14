using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class SphereArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new();
        private readonly List<Quaternion> _rotations = new();
        private readonly Vector3 _center;

        public SphereArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale, packingFactor);

            _positions.Add(Vector3.zero);
            _rotations.Add(Quaternion.identity);

            var shell = 1;
            while (_positions.Count < count)
            {
                var shellRadius = shell * spacing;

                var surfaceArea = 4 * Mathf.PI * Mathf.Pow(shellRadius, 2);
                var pointsOnShell = Mathf.FloorToInt(surfaceArea / Mathf.Pow(spacing, 2));

                for (int i = 0; i < pointsOnShell && _positions.Count < count; i++)
                {
                    var phi = Mathf.Acos(1 - 2 * (i + 0.5f) / pointsOnShell);
                    var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

                    var x = shellRadius * Mathf.Sin(phi) * Mathf.Cos(theta);
                    var y = shellRadius * Mathf.Cos(phi);
                    var z = shellRadius * Mathf.Sin(phi) * Mathf.Sin(theta);

                    _rotations.Add(Quaternion.Euler(_positions.Count * 47 % 360, _positions.Count * 31 % 360, _positions.Count * 13 % 360));
                    _positions.Add(new Vector3(x, y, z));
                    
                }

                shell++;
            }

            var minY = _positions.Min(p => p.y);
            var offsetVector = new Vector3(0, -minY + offset, 0);

            _positions = _positions.Select(p => p + offsetVector).ToList();
        }

        public List<Vector3> GetVectors()
        {
            return _positions;
        }

        public List<float3> GetFloats()
        {
            return _positions.Select(x => (float3)x).ToList();
        }

        public List<Quaternion> GetRotations()
        {
            return _rotations;
        }
    }
}