using System.Diagnostics;
using System.Threading;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using IterationTest.ECSCommon;
using IterationTest.ECSMainThread;
using Unity.Entities;
using VContainer.Unity;
using CleanupSystem = IterationTest.ECSCommon.CleanupSystem;
using SetupSystem = IterationTest.ECSCommon.SetupSystem;

namespace IterationTest.ECSBurst
{
    public class TestLogic : IAsyncStartable
    {
        private readonly TestResults _testResults = new();
        private readonly TestManager _testManager;
        private readonly IUprofWrapper _uprofWrapper;
        private readonly EcsIterationBurst _testCase;
        private readonly TestHudLogic _testHudLogic;

        public TestLogic(TestManager testManager, IUprofWrapper uprofWrapper,
            ITestCaseFactory<EcsIterationBurst> defaultFactory, TestHudLogic hudLogic)
        {
            _testManager = testManager;
            _uprofWrapper = uprofWrapper;

            _testCase = testManager.GetOrCreateTestCase(defaultFactory);
            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.TestCase = nameof(EcsIterationBurst);
            _testHudLogic = hudLogic;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            _testHudLogic.SetTitle(nameof(EcsIterationBurst));

            var totalTime = new Stopwatch();
            var stopwatch = new Stopwatch();

            _testManager.PublishMessage($"N = {_testCase.Count}");

            await UniTask.Yield();

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

            _testManager.PublishMessage("Setup...");

            await UniTask.Yield();

            totalTime.Start();

            var setupSystem = world.CreateSystem<SetupSystem>();

            stopwatch.Start();
            setupSystem.Update(world.Unmanaged);
            stopwatch.Stop();
            _testResults.KeyValues["Setup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Execution...");

            await UniTask.Yield();

            var executionSystem = world.CreateSystem<ExecutionSystem>();

            await _uprofWrapper.StartProfiling();
            stopwatch.Start();
            executionSystem.Update(world.Unmanaged);
            stopwatch.Stop();


            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);

            _testResults.KeyValues["Execution"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Cleanup...");

            await UniTask.Yield();

            var cleanupSystem = world.CreateSystem<CleanupSystem>();
            stopwatch.Start();
            cleanupSystem.Update(world.Unmanaged);
            stopwatch.Stop();
            _testResults.KeyValues["Cleanup"] = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            _testManager.PublishMessage("Finished...");

            await UniTask.Yield();

            totalTime.Stop();

            world.DestroyAllSystemsAndLogException(out _);
            world.Dispose();
            
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