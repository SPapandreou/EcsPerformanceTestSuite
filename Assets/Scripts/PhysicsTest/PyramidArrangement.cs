using System.Collections.Generic;
using System.Linq;
using Core;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class PyramidArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new ();
        private readonly List<Quaternion> _rotations = new ();
        
        public PyramidArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale, packingFactor);

            var layers = Mathf.CeilToInt(Mathf.Pow(3 * count, 1f / 3f));

            var generated = 0;
            for (var layer = 0; generated < count; layer++)
            {
                var layerWidth = layer + 1;
                var start = -(layerWidth - 1) * spacing * 0.5f;
                var y = (layers - 1 - layer) * spacing;

                for (var i = 0; i < layerWidth && generated < count; i++)
                {
                    for (var j = 0; j < layerWidth && generated < count; j++)
                    {
                        var x = start + i * spacing;
                        var z = start + j * spacing;
                        var position = new Vector3(x, y, z);

                        _positions.Add(position);
                        _rotations.Add(Quaternion.Euler(generated * 47 % 360, generated * 31 % 360, generated * 13 % 360));
                        generated++;
                    }
                }
            }

            var offsetVector = new Vector3(0, layers * spacing + offset, 0);
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