using System;
using System.Threading;
using Core.EcsWorld;
using Core.Shapes;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using Unity.Transforms;
using UnityEngine;
using VContainer.Unity;

namespace PhysicsTest.ECS
{
    public class TestLogic : IAsyncStartable
    {
        private readonly GameObject[] _prefabs;
        private readonly TestManager _testManager;
        private readonly EcsPhysics _testCase;
        private readonly TestHudLogic _testHudLogic;
        private readonly FpsCounter _fpsCounter;
        private readonly TestResults _testResults = new();
        private readonly Camera _camera;
        private readonly IUprofWrapper _uprofWrapper;
        private readonly WorldContainer _worldContainer;


        public TestLogic(TestManager testManager, WorldContainer worldContainer,
            ITestCaseFactory<EcsPhysics> testCaseFactory, TestHudLogic testHudLogic, FpsCounter fpsCounter,
            Camera camera, IUprofWrapper uprofWrapper)
        {
            _worldContainer = worldContainer;
            _uprofWrapper = uprofWrapper;
            _camera = camera;
            _testManager = testManager;
            _testCase = _testManager.GetOrCreateTestCase(testCaseFactory);
            _testHudLogic = testHudLogic;
            _fpsCounter = fpsCounter;

            _testResults.TestCase = nameof(EcsPhysics);

            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.Parameters["PrimitiveShape"] = _testCase.PrimitiveShape.ToString();
            _testResults.Parameters["ArrangementShape"] = _testCase.ArrangementShape.ToString();
            _testResults.Parameters["HeightOffset"] = _testCase.HeightOffset;
            _testResults.Parameters["PackingFactor"] = _testCase.PackingFactor;
            _testResults.Parameters["Scale"] = _testCase.Scale;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            Physics.simulationMode = SimulationMode.Script;

            _testManager.PublishMessage($"N = {_testCase.Count}");
            
            _testHudLogic.SetTitle(nameof(EcsPhysics));
            _testManager.PublishMessage("Setup...");

            await UniTask.Yield();

            var generator = _testCase.ArrangementShape.GetGenerator(_testCase.Count, _testCase.PrimitiveShape,
                _testCase.Scale, _testCase.HeightOffset, _testCase.PackingFactor);
            var positions = generator.GetFloats();
            var rotations = generator.GetEcsRotations();

            var entityManager = _worldContainer.World.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PhysicsTestData));

            while (query.CalculateEntityCount() == 0)
            {
                await UniTask.Yield();
            }

            var physicsTestData = query.GetSingleton<PhysicsTestData>();

            var prefab = _testCase.PrimitiveShape switch
            {
                PrimitiveShape.Capsule => physicsTestData.Capsule,
                PrimitiveShape.Sphere => physicsTestData.Sphere,
                PrimitiveShape.Cube => physicsTestData.Cube,
                _ => throw new ArgumentOutOfRangeException()
            };
            

            for (var i = 0; i < positions.Count; i++)
            {
                var entity = entityManager.Instantiate(prefab);
                entityManager.SetComponentData(entity,
                    LocalTransform.FromPositionRotationScale(positions[i], rotations[i], _testCase.Scale));
            }

            generator.SetCameraPosition(_camera);

            _testManager.PublishMessage("Execution...");
            _worldContainer.StopUpdateWorld();

            await UniTask.NextFrame();

            if (!_testCase.Warmup)
            {
                for (int i = 3; i > 0; i--)
                {
                    _testManager.PublishMessage($"{i}...");
                    await UniTask.Delay(1000, cancellationToken: cancellation);
                }
            }

            await _uprofWrapper.StartProfiling();
            _testManager.PublishMessage("Start...");
            _worldContainer.UpdateWorld();

            _fpsCounter.Start();

            if (_testCase.Warmup)
            {
                await UniTask.NextFrame();
                _fpsCounter.Stop();
                _testCase.TestFinished();
                return;
            }

            _testHudLogic.SetFinishTestEnabled(true);
            await _testHudLogic.WaitForFinish();
            _fpsCounter.Stop();

            _worldContainer.StopUpdateWorld();

            _testManager.PublishMessage("Cleanup...");
            await UniTask.Yield();
            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);

            _testResults.KeyValues["AverageFps"] = _fpsCounter.AverageFps;
            _testResults.KeyValues["MinFps"] = _fpsCounter.MinFps;
            _testResults.KeyValues["MaxFps"] = _fpsCounter.MaxFps;
            _testResults.KeyValues["Time"] = _fpsCounter.TotalTime;
            _testResults.KeyValues["TotalFrames"] = _fpsCounter.TotalFrames;

            _testResults.TimeSeriesData["Fps"] = _fpsCounter.GetFpsTimeSeries();

            _testResults.WriteToFile(_testCase.OutputDirectory);

            _testCase.TestFinished();
        }
    }
}