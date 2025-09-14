using AnimationTest.ECS;
using AnimationTest.OOP;
using Core.Tests;
using UnityEngine;

namespace AnimationTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<EcsAnimationTestCase>,
        ITestCaseFactory<OopAnimationTestCase>
    {
        public int count;
        public float duration;
        EcsAnimationTestCase ITestCaseFactory<EcsAnimationTestCase>.CreateTestCase()
        {
            return new EcsAnimationTestCase
            {
                Count = count,
                Duration = duration
            };
        }

        OopAnimationTestCase ITestCaseFactory<OopAnimationTestCase>.CreateTestCase()
        {
            return new OopAnimationTestCase
            {
                Count = count,
                Duration = duration
            };
        }
    }
}