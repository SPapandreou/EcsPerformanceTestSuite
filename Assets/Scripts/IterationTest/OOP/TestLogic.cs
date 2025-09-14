using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Core.Configuration;
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
        private readonly UprofWrapper _uprofWrapper;
        private readonly OopIterationTestCase _testCase;

        public TestLogic(TestManager testManager, UprofWrapper uprofWrapper,
            ITestCaseFactory<OopIterationTestCase> defaultFactory)
        {
            _testManager = testManager;
            _uprofWrapper = uprofWrapper;
            _testCase = testManager.GetOrCreateTestCase(defaultFactory);

            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.TestCase = nameof(OopIterationTestCase);
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            var totalTime = new Stopwatch();
            var stopwatch = new Stopwatch();

            _testManager.PublishMessage("==== Starting execution of OOP Iteration Test ====");
            _testManager.PublishMessage($"Size: {_testCase.Count}");

            totalTime.Start();


            _testManager.PublishMessage("Setup...");

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

            _testManager.PublishMessage("Setup finished...");

            await UniTask.Yield();

            _testManager.PublishMessage("Execution...");

            var velocity = IterationTestConfiguration.VelocityVector;

            await _uprofWrapper.StartProfiling();
            stopwatch.Start();
            foreach (var gameObject in gameObjects)
            {
                gameObject.transform.position += velocity;
            }

            stopwatch.Stop();

            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);
            _testResults.KeyValues["Execution"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Execution finished...");

            await UniTask.Yield();

            _testManager.PublishMessage("Cleanup...");

            stopwatch.Start();
            foreach (var gameObject in gameObjects)
            {
                Object.Destroy(gameObject);
            }

            gameObjects.Clear();

            stopwatch.Stop();
            _testResults.KeyValues["Cleanup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Cleanup finished...");

            await UniTask.Yield();

            totalTime.Stop();
            _testResults.KeyValues["WallTime"] = totalTime.Elapsed.TotalSeconds;
            _testManager.PublishMessage("==== Execution of OOP Iteration Test finished ====");
            _testManager.PublishMessage("==== Results: ====");
            _testManager.PublishMessage(_testResults.ToString());
            _testResults.WriteToFile(_testCase.OutputDirectory);

            _testCase.TestFinished();
        }
    }
}