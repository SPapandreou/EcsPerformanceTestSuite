using AnimationTest.ECS;
using AnimationTest.OOP;
using Core.Tests;
using UnityEngine;

namespace AnimationTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<EcsAnimation>,
        ITestCaseFactory<OopAnimation>
    {
        public int count;
        public float duration;
        EcsAnimation ITestCaseFactory<EcsAnimation>.CreateTestCase()
        {
            return new EcsAnimation
            {
                Count = count,
                Duration = duration
            };
        }

        OopAnimation ITestCaseFactory<OopAnimation>.CreateTestCase()
        {
            return new OopAnimation
            {
                Count = count,
                Duration = duration
            };
        }
    }
}