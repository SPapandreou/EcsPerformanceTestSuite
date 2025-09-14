using System;
using System.Threading;
using AnimationTest.OOP;
using Core;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using Latios.Transforms;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using UnityEngine;
using VContainer.Unity;

namespace AnimationTest.ECS
{
    public class TestLogic : IAsyncStartable, IDisposable
    {
        private readonly TestResults _testResults = new();
        private readonly FpsCounter _fpsCounter;
        private readonly TestManager _testManager;
        private readonly Camera _mainCamera;
        private readonly CinemachinePositionComposer _positionComposer;
        private readonly WorldContainer _worldContainer;
        private readonly EcsAnimationTestCase _testCase;
        private readonly UprofWrapper _uprofWrapper;

        public TestLogic(WorldContainer worldContainer, FpsCounter fpsCounter, TestManager testManager,
            Camera mainCamera, CinemachinePositionComposer positionComposer, ITestCaseFactory<EcsAnimationTestCase> defaultFactory, UprofWrapper uprofWrapper)
        {
            _fpsCounter = fpsCounter;
            _testManager = testManager;
            _mainCamera = mainCamera;
            _positionComposer = positionComposer;
            _worldContainer = worldContainer;
            _uprofWrapper = uprofWrapper;

            _testCase = _testManager.GetOrCreateTestCase(defaultFactory);
            
            _testCase = _testManager.GetOrCreateTestCase(defaultFactory);
            _testResults.TestCase = nameof(EcsAnimationTestCase);
            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.Parameters["Duration"] = _testCase.Duration;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            _testManager.PublishMessage("==== Starting execution of ECS Animation Test ====");
            _testManager.PublishMessage($"Size: {_testCase.Count}");
            _testManager.PublishMessage($"Duration: {_testCase.Duration}");

            var query = _worldContainer.World.EntityManager.CreateEntityQuery(typeof(AnimationTestConfigData));

            while (query.CalculateEntityCount() == 0)
            {
                await UniTask.NextFrame();
            }

            var config = query.GetSingleton<AnimationTestConfigData>();

            var columns = Mathf.CeilToInt(Mathf.Sqrt(_testCase.Count));
            var rows = Mathf.CeilToInt((float)_testCase.Count / columns);

            var centerX = (columns - 1) / 2;
            var centerY = (rows - 1) / 2;

            for (var i = 0; i < _testCase.Count; i++)
            {
                var row = i / columns;
                var col = i % columns;

                var position = new float3(col, 0, row);
                var peter = _worldContainer.World.EntityManager.Instantiate(config.PeterPrefab);
                var transform = _worldContainer.World.EntityManager.GetAspect<TransformAspect>(peter);
                transform.worldPosition = position;

                if (col == centerX && row == centerY)
                {
                    _worldContainer.World.EntityManager.AddComponent<CenterPeterTag>(peter);
                }
            }

            var xRotation = _positionComposer.VirtualCamera.transform.eulerAngles.x * Mathf.Deg2Rad;
            var fov = _mainCamera.fieldOfView * Mathf.Deg2Rad;
            var angle = xRotation + fov / 2;

            var halfDiagonal = rows / 2f * Mathf.Sqrt(2);


            var distance = halfDiagonal / Mathf.Sin(angle);
            distance *= 1.5f;

            _positionComposer.CameraDistance = distance;

            _worldContainer.World.EntityManager.CreateSingleton(new AnimationTestStateData
            {
                State = PeterState.Walking,
                TransitionTime = -1f,
                Remaining = config.WalkDistance
            });

            await UniTask.NextFrame();


            var system = _worldContainer.World.CreateSystem<PeterSystem>();
            var group = _worldContainer.World.GetExistingSystemManaged<SimulationSystemGroup>();

            group.AddSystemToUpdateList(system);
            group.SortSystems();
            PrintSystems(_worldContainer);

            await UniTask.NextFrame();


            _fpsCounter.Start();

            while (_fpsCounter.TotalTime < _testCase.Duration)
            {
                await UniTask.NextFrame();
            }

            _fpsCounter.Stop();

            _testResults.KeyValues["AverageFps"] = _fpsCounter.AverageFps;
            _testResults.KeyValues["MinFps"] = _fpsCounter.MinFps;
            _testResults.KeyValues["MaxFps"] = _fpsCounter.MaxFps;
            
            _testResults.WriteToFile(_testCase.OutputDirectory);
            _testCase.TestFinished();
        }

        private static void PrintSystems(WorldContainer container)
        {
            var group = container.World.GetExistingSystemManaged<SimulationSystemGroup>();
            foreach (var test in group.GetAllSystems())
            {
                ref readonly var state = ref container.World.Unmanaged.ResolveSystemStateRef(test);
                Debug.Log(state.DebugName);
            }
        }

        public void Dispose()
        {
            _fpsCounter.Dispose();
        }
    }
}