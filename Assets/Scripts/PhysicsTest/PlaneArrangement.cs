using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Shapes;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsTest
{
    public class PlaneArrangement : IArrangementGenerator
    {
        private readonly List<Vector3> _positions = new();
        private readonly List<Quaternion> _rotations = new();
        
        public PlaneArrangement(int count, PrimitiveShape shape, float scale, float offset, float packingFactor)
        {
            var spacing = shape.GetSpacing(scale, packingFactor);

            // Determine grid dimensions (approx square)
            var columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            var rows = Mathf.CeilToInt((float)count / columns);

            var generated = 0;
            for (var x = 0; x < columns && generated < count; x++)
            {
                for (var z = 0; z < rows && generated < count; z++)
                {
                    var position = new Vector3(x * spacing, offset, z * spacing);
                    _positions.Add(position);
                    _rotations.Add(Quaternion.Euler(generated * 47 % 360, generated * 31 % 360, generated * 13 % 360));
                    generated++;
                }
            }

            // Center the plane
            var offsetVector = new Vector3((columns - 1) * spacing / 2f, 0, (rows - 1) * spacing / 2f);
            _positions = _positions.Select(p => p - offsetVector).ToList();
            
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
            // Compute bounds in XZ plane
            var min = _positions.Aggregate(Vector3.positiveInfinity, Vector3.Min);
            var max = _positions.Aggregate(Vector3.negativeInfinity, Vector3.Max);
    
            var center = (min + max) * 0.5f;
            var size = max - min;
            var horizontalExtent = Mathf.Sqrt(size.x * size.x + size.z * size.z) * 0.5f;

            // Use FOV to compute distance (vertical FOV)
            var fov = camera.fieldOfView * Mathf.Deg2Rad;
            var distance = horizontalExtent / Mathf.Tan(fov * 0.5f);
            distance *= 1.2f;

            var tiltAngle = 45f; // tilt from horizontal
            var azimuth = 45f;   // rotation around Y

            var direction = Quaternion.Euler(tiltAngle, azimuth, 0f) * Vector3.back;

            camera.transform.position = center + direction.normalized * distance;
            camera.transform.LookAt(center);
        }
    }
}