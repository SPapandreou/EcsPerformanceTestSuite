using System.Diagnostics;
using System.IO;
using System.Threading;
using Core;
using Core.Configuration;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using VContainer.Unity;
using Debug = UnityEngine.Debug;

namespace IterationTest.ECS
{
    public class TestLogic : IAsyncStartable
    {
        private readonly TestResults _testResults = new();
        private readonly TestManager _testManager;
        private readonly UprofWrapper _uprofWrapper;
        private readonly EcsIterationTestCase _testCase;

        public TestLogic(TestManager testManager, UprofWrapper uprofWrapper,
            ITestCaseFactory<EcsIterationTestCase> defaultFactory)
        {
            _testManager = testManager;
            _uprofWrapper = uprofWrapper;

            _testCase = testManager.GetOrCreateTestCase(defaultFactory);
            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.TestCase = nameof(EcsIterationTestCase);
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            var totalTime = new Stopwatch();
            var stopwatch = new Stopwatch();

            _testManager.PublishMessage("==== Starting execution of ECS Iteration Test ====");
            _testManager.PublishMessage($"Size: {_testCase.Count}");
            
            _testManager.PublishMessage("Creating World...");
            var world = new World("EcsIterationTest");

            world.EntityManager.CreateSingleton(new IterationTestData
            {
                Size = _testCase.Count,
                X = IterationTestConfiguration.X,
                Y = IterationTestConfiguration.Y,
                Z = IterationTestConfiguration.Z,
                Velocity = IterationTestConfiguration.VelocityFloat
            });

            totalTime.Start();

            _testManager.PublishMessage("Setup...");

            var setupSystem = world.CreateSystem<SetupSystem>();

            stopwatch.Start();
            setupSystem.Update(world.Unmanaged);
            stopwatch.Stop();
            _testResults.KeyValues["Setup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Setup finished...");

            await UniTask.Yield();

            _testManager.PublishMessage("Execution...");

            var executionSystem = world.CreateSystem<ExecutionSystem>();

            await _uprofWrapper.StartProfiling();
            stopwatch.Start();
            executionSystem.Update(world.Unmanaged);
            stopwatch.Stop();


            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);

            _testResults.KeyValues["Execution"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Execution finished...");

            await UniTask.Yield();

            _testManager.PublishMessage("Cleanup...");

            var cleanupSystem = world.CreateSystem<CleanupSystem>();
            stopwatch.Start();
            cleanupSystem.Update(world.Unmanaged);
            stopwatch.Stop();
            _testResults.KeyValues["Cleanup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Cleanup finished...");

            await UniTask.Yield();

            totalTime.Stop();
            _testResults.KeyValues["WallTime"] = totalTime.Elapsed.TotalSeconds;
            _testManager.PublishMessage("==== Execution of ECS Iteration Test finished ====");
            _testManager.PublishMessage("==== Results: ====");
            _testManager.PublishMessage(_testResults.ToString());

            world.DestroyAllSystemsAndLogException(out _);
            world.Dispose();
            _testResults.WriteToFile(_testCase.OutputDirectory);

            _testCase.TestFinished();
        }
    }
}