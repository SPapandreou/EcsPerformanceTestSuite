using Core;
using Core.Shapes;
using Core.Tests;
using PhysicsTest.ECS;
using PhysicsTest.OOP;
using UnityEngine;

namespace PhysicsTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<OopPhysics>, ITestCaseFactory<EcsPhysics>
    {
        public int count;
        public ArrangementShape arrangementShape;
        public PrimitiveShape primitiveShape;
        public float scale;
        public float offset;
        public float packingFactor;

        public OopPhysics CreateTestCase()
        {
            return new OopPhysics
            {
                ArrangementShape = arrangementShape,
                PrimitiveShape = primitiveShape,
                Count = count,
                ExitAfterExecution = true,
                HeightOffset = offset,
                PackingFactor = packingFactor,
                Scale = scale
            };
        }

        EcsPhysics ITestCaseFactory<EcsPhysics>.CreateTestCase()
        {
            return new EcsPhysics
            {
                ArrangementShape = arrangementShape,
                PrimitiveShape = primitiveShape,
                Count = count,
                ExitAfterExecution = true,
                HeightOffset = offset,
                PackingFactor = packingFactor,
                Scale = scale
            };
        }
    }
}