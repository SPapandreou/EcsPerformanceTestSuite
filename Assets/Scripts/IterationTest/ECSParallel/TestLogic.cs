using System.Diagnostics;
using System.Threading;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using IterationTest.ECSCommon;
using Unity.Entities;
using VContainer.Unity;

namespace IterationTest.ECSParallel
{
    public class TestLogic : IAsyncStartable
    {
        private readonly TestResults _testResults = new();
        private readonly TestManager _testManager;
        private readonly IUprofWrapper _uprofWrapper;
        private readonly EcsIterationParallel _testCase;
        private readonly TestHudLogic _testHudLogic;

        public TestLogic(TestManager testManager, IUprofWrapper uprofWrapper,
            ITestCaseFactory<EcsIterationParallel> defaultFactory, TestHudLogic hudLogic)
        {
            _testManager = testManager;
            _uprofWrapper = uprofWrapper;

            _testCase = testManager.GetOrCreateTestCase(defaultFactory);
            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.TestCase = nameof(EcsIterationParallel);
            _testHudLogic =  hudLogic;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            _testHudLogic.SetTitle(nameof(EcsIterationParallel));
            
            var totalTime = new Stopwatch();
            var stopwatch = new Stopwatch();
            
            _testManager.PublishMessage($"N = {_testCase.Count}");
            _testManager.PublishMessage($"i = {_testCase.Iterations}");

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

            if (!_testCase.Warmup)
            {
                for (int i = 0; i < 5; i++)
                {
                    executionSystem.Update(world.Unmanaged);
                }
            }

            await _uprofWrapper.StartProfiling();
            stopwatch.Start();
            
            for (int i = 0; i < _testCase.Iterations; i++)
            {
                executionSystem.Update(world.Unmanaged);
            }
            
            
            stopwatch.Stop();


            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);

            _testResults.KeyValues["Execution"] = stopwatch.Elapsed.TotalSeconds/_testCase.Iterations;
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
            _testResults.KeyValues["WallTime"] = totalTime.Elapsed.TotalSeconds;
            
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