using Core.Tests;
using IterationTest.ECS;
using IterationTest.OOP;
using UnityEngine;

namespace IterationTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<OopIterationTestCase>, ITestCaseFactory<EcsIterationTestCase>
    {
        public int count;
        OopIterationTestCase ITestCaseFactory<OopIterationTestCase>.CreateTestCase()
        {
            return new OopIterationTestCase
            {
                Count = count
            };
        }

        EcsIterationTestCase ITestCaseFactory<EcsIterationTestCase>.CreateTestCase()
        {
            return new EcsIterationTestCase
            {
                Count = count
            };
        }
    }
}