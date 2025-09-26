using Core.Tests;
using IterationTest.ECSBurst;
using IterationTest.ECSMainThread;
using IterationTest.ECSMathBurst;
using IterationTest.ECSMathParallel;
using IterationTest.ECSParallel;
using IterationTest.OOP;
using UnityEngine;

namespace IterationTest
{
    public class DefaultParameters : MonoBehaviour, ITestCaseFactory<OopIteration>, ITestCaseFactory<EcsIterationMainThread>, ITestCaseFactory<EcsIterationBurst>, ITestCaseFactory<EcsIterationParallel>, ITestCaseFactory<EcsIterationMathBurst>, ITestCaseFactory<EcsIterationMathParallel>
    {
        public int count;
        public int iterations;
        
        OopIteration ITestCaseFactory<OopIteration>.CreateTestCase()
        {
            return new OopIteration
            {
                Count = count,
                Iterations = iterations
            };
        }
        
        EcsIterationMainThread ITestCaseFactory<EcsIterationMainThread>.CreateTestCase()
        {
            return new EcsIterationMainThread
            {
                Count = count,
                Iterations = iterations
            };
        }

        EcsIterationBurst ITestCaseFactory<EcsIterationBurst>.CreateTestCase()
        {
            return new EcsIterationBurst
            {
                Count = count,
                Iterations = iterations
            };
        }

        EcsIterationParallel ITestCaseFactory<EcsIterationParallel>.CreateTestCase()
        {
            return new EcsIterationParallel
            {
                Count = count,
                Iterations = iterations
            };
        }

        EcsIterationMathBurst ITestCaseFactory<EcsIterationMathBurst>.CreateTestCase()
        {
            return new EcsIterationMathBurst
            {
                Count = count,
                Iterations = iterations
            };
        }

        EcsIterationMathParallel ITestCaseFactory<EcsIterationMathParallel>.CreateTestCase()
        {
            return new EcsIterationMathParallel
            {
                Count = count,
                Iterations = iterations
            };
        }
    }
}