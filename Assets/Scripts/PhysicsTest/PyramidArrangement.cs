using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Shapes;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class PyramidArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new ();
        private readonly List<Quaternion> _rotations = new ();

        private readonly float _offset;

        private readonly Bounds _bounds;
        
        public PyramidArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            _offset = offset;
            
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
            
            var offsetVector = new Vector3(0, offset+2*scale, 0);
            _positions = _positions.Select(p => p + offsetVector).ToList();
            
            var min = _positions.Aggregate(Vector3.positiveInfinity, Vector3.Min);
            var max = _positions.Aggregate(Vector3.negativeInfinity, Vector3.Max);
            _bounds = new Bounds(min, max);
            _bounds.Encapsulate(new Vector3(_bounds.center.x, 0f, _bounds.center.z));
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
        
        public List<quaternion> GetEcsRotations()
        {
            return _rotations.Select(r => (quaternion)r).ToList();
        }

        public void SetCameraPosition(Camera camera)
        {
            // Look target is the center of bounds, but raise slightly so ground is visible
            var lookAt = _bounds.center;
            lookAt.y = _bounds.max.y * 0.5f;

            // Calculate bounding sphere radius
            var extents = _bounds.extents;
            var radius = extents.magnitude; // conservative: fits everything

            // Use camera FOV and aspect ratio to compute required distance
            var fovY = camera.fieldOfView * Mathf.Deg2Rad;
            var fovX = 2f * Mathf.Atan(Mathf.Tan(fovY / 2f) * camera.aspect);

            // Required distances for vertical and horizontal FOV
            var distanceY = radius / Mathf.Sin(fovY / 2f);
            var distanceX = radius / Mathf.Sin(fovX / 2f);

            var distance = Mathf.Max(distanceX, distanceY);
            distance *= 1.2f;

            // Camera orientation
            var tiltAngle = 30f;
            var azimuth = 45f;
            var direction = Quaternion.Euler(tiltAngle, azimuth, 0f) * Vector3.back;

            // Place camera
            camera.transform.position = lookAt + direction.normalized * distance;
            camera.transform.LookAt(lookAt);
        }
    }
}