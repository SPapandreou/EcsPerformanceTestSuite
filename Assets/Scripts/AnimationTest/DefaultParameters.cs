using AnimationTest.ECS;
using AnimationTest.ECSBurst;
using AnimationTest.ECSMainThread;
using AnimationTest.OOP;
using Core.Tests;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace AnimationTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<EcsAnimation>, ITestCaseFactory<EcsAnimationBurst>, ITestCaseFactory<EcsAnimationMainThread>,
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

        EcsAnimationBurst ITestCaseFactory<EcsAnimationBurst>.CreateTestCase()
        {
            return new EcsAnimationBurst
            {
                Count = count,
                Duration = duration
            };
        }

        EcsAnimationMainThread ITestCaseFactory<EcsAnimationMainThread>.CreateTestCase()
        {
            return new EcsAnimationMainThread
            {
                Count = count,
                Duration = duration
            };
        }
    }
}