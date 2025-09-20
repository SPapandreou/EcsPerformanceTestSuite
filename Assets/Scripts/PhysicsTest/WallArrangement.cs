using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Shapes;
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
        
        public List<quaternion> GetEcsRotations()
        {
            return _rotations.Select(r => (quaternion)r).ToList();
        }

        public void SetCameraPosition(Camera camera)
        {
            // Calculate bounds of wall
            var min = _positions.Aggregate(Vector3.positiveInfinity, Vector3.Min);
            var max = _positions.Aggregate(Vector3.negativeInfinity, Vector3.Max);
            var bounds = new Bounds((min + max) * 0.5f, max - min);
            bounds.Encapsulate(new Vector3(bounds.center.x, 0f, bounds.center.z)); // include ground

            // Look at center of wall
            var lookAt = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z);

            // Field of view in radians
            var fov = camera.fieldOfView * Mathf.Deg2Rad;

            // Use the largest extent to calculate distance
            var verticalDistance = bounds.extents.y / Mathf.Tan(fov * 0.5f);
            var horizontalFov = 2 * Mathf.Atan(Mathf.Tan(fov * 0.5f) * camera.aspect);
            var horizontalDistance = bounds.extents.x / Mathf.Tan(horizontalFov * 0.5f);

            var distance = Mathf.Max(verticalDistance, horizontalDistance) * 1.2f; // add margin

            // Tilt and azimuth
            var tiltAngle = 30f;
            var azimuth = 45f;

            var direction = Quaternion.Euler(tiltAngle, azimuth, 0f) * Vector3.back;
            camera.transform.position = lookAt + direction.normalized * distance;
            camera.transform.LookAt(lookAt);
        }
    }
}