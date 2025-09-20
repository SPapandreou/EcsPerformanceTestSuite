using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Shapes;
using Unity.Mathematics;
using Unity.Physics;
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

            var size = Mathf.FloorToInt(Mathf.Pow(count, 1f / 3f));
            
            if (Mathf.Pow(size, 3) < count)
            {
                size++;
            }
            
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

            var offsetVector = new Vector3(
                -((size - 1) * spacing) / 2f,     
                offset + scale * 2f,            
                -((size - 1) * spacing) / 2f);
            
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

        public List<quaternion> GetEcsRotations()
        {
            return _rotations.Select(r => (quaternion)r).ToList();
        }

        public void SetCameraPosition(Camera camera)
        {
            var min = _positions.Aggregate(Vector3.positiveInfinity, Vector3.Min);
            var max = _positions.Aggregate(Vector3.negativeInfinity, Vector3.Max);
            var bounds = new Bounds((min + max) * 0.5f, max - min);
            bounds.Encapsulate(new Vector3(bounds.center.x, 0f, bounds.center.z));
            
            // Look at the center of the bounds
            var lookAt = bounds.center;

            var radius = bounds.extents.magnitude;
            var fov = camera.fieldOfView * Mathf.Deg2Rad;
            var horizontalFov = 2f * Mathf.Atan(Mathf.Tan(fov * 0.5f) * camera.aspect);

            var halfAngle = Mathf.Min(fov, horizontalFov) * 0.5f;
            
            var distance = radius / Mathf.Sin(halfAngle);
            distance *= 1.2f; // add margin

            // Camera angles
            var tiltAngle = 30f;   // tilt down
            var azimuth = 45f;     // rotated for perspective

            var direction = Quaternion.Euler(tiltAngle, azimuth, 0f) * Vector3.back;
            camera.transform.position = lookAt + direction.normalized * distance;
            camera.transform.LookAt(lookAt);
        }
    }
}