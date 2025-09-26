using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace IterationTest.OOP
{
    public class TestLogic : IAsyncStartable
    {
        private readonly TestResults _testResults = new();
        private readonly TestManager _testManager;
        private readonly IUprofWrapper _uprofWrapper;
        private readonly OopIteration _testCase;
        private readonly TestHudLogic _testHudLogic;

        public TestLogic(TestManager testManager, IUprofWrapper uprofWrapper,
            ITestCaseFactory<OopIteration> defaultFactory, TestHudLogic testHudLogic)
        {
            _testManager = testManager;
            _uprofWrapper = uprofWrapper;
            _testCase = testManager.GetOrCreateTestCase(defaultFactory);

            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.TestCase = nameof(OopIteration);
            _testHudLogic = testHudLogic;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            _testHudLogic.SetTitle(nameof(OopIteration));
            _testHudLogic.SetFpsEnabled(false);
            _testManager.PublishMessage($"N = {_testCase.Count}");
            _testManager.PublishMessage($"i = {_testCase.Iterations}");
            
            var totalTime = new Stopwatch();
            var stopwatch = new Stopwatch();

            await UniTask.Yield();

            totalTime.Start();
            
            _testManager.PublishMessage("Setup...");
            
            await UniTask.Yield();

            var x = IterationTestConfiguration.X;
            var y = IterationTestConfiguration.Y;
            var z = IterationTestConfiguration.Z;

            var positions = new List<Vector3>();
            var currentPosition = Vector3.zero;


            stopwatch.Start();
            for (int i = 0; i < _testCase.Count; i++)
            {
                positions.Add(currentPosition);
                currentPosition += new Vector3(x, y, z);
                (x, y, z) = (z, x, y);
            }

            var gameObjects = new List<GameObject>();

            for (int i = 0; i < _testCase.Count; i++)
            {
                var gameObject = new GameObject();
                gameObject.transform.position = positions[i];
                gameObjects.Add(gameObject);
            }

            stopwatch.Stop();
            _testResults.KeyValues["Setup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            
            _testManager.PublishMessage("Execution...");

            await UniTask.Yield();

            var velocity = IterationTestConfiguration.VelocityVector;
            
            for (int i = 0; i < 5; i++)
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.transform.position += velocity;
                }    
            }

            await _uprofWrapper.StartProfiling();
            stopwatch.Start();
            
            for (int i = 0; i < _testCase.Iterations; i++)
            {
                foreach (var gameObject in gameObjects)
                {
                    gameObject.transform.position += velocity;
                }    
            }
            

            stopwatch.Stop();

            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);
            _testResults.KeyValues["Execution"] = stopwatch.Elapsed.TotalSeconds/_testCase.Iterations;
            stopwatch.Reset();

            _testManager.PublishMessage("Cleanup...");
            
            await UniTask.Yield();

            stopwatch.Start();
            foreach (var gameObject in gameObjects)
            {
                Object.Destroy(gameObject);
            }

            gameObjects.Clear();

            stopwatch.Stop();
            _testResults.KeyValues["Cleanup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Finished...");
            
            await UniTask.Yield();
            
            totalTime.Stop();
            _testResults.KeyValues["WallTime"] = totalTime.Elapsed.TotalSeconds;

            
            if (_testCase.Warmup)
            {
                _testCase.TestFinished();
                return;
            }
            _testResults.WriteToFile(_testCase.OutputDirectory);

            await UniTask.Delay(2000, cancellationToken: cancellation);

            _testCase.TestFinished();
        }
    }
}