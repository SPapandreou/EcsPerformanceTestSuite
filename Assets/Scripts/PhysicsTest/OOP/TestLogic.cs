using System;
using System.Threading;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace PhysicsTest.OOP
{
    public class TestLogic : IAsyncStartable, IDisposable
    {
        private readonly GameObject[] _prefabs;
        private readonly TestManager _testManager;
        private readonly OopPhysics _testCase;
        private readonly TestHudLogic _testHudLogic;
        private readonly FpsCounter _fpsCounter;
        private readonly TestResults _testResults = new();
        private readonly Camera _camera;
        private readonly IUprofWrapper _uprofWrapper;

        public TestLogic(GameObject[] prefabs, TestManager testManager,
            ITestCaseFactory<OopPhysics> testCaseFactory, TestHudLogic testHudLogic, FpsCounter fpsCounter, Camera camera, IUprofWrapper uprofWrapper)
        {
            _uprofWrapper = uprofWrapper;
            _camera = camera;
            _prefabs = prefabs;
            _testManager = testManager;
            _testCase = _testManager.GetOrCreateTestCase(testCaseFactory);
            _testHudLogic = testHudLogic;
            _fpsCounter = fpsCounter;

            _testResults.TestCase = nameof(OopPhysics);
            
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
            
            _testHudLogic.SetTitle(nameof(OopPhysics));
            _testManager.PublishMessage($"N = {_testCase.Count}");
            _testManager.PublishMessage("Setup...");

            await UniTask.Yield();

            var generator = _testCase.ArrangementShape.GetGenerator(_testCase.Count, _testCase.PrimitiveShape,
                _testCase.Scale, _testCase.HeightOffset, _testCase.PackingFactor);
            var positions = generator.GetVectors();
            var rotations = generator.GetRotations();

            for (var i = 0; i < positions.Count; i++)
            {
                var gameObject =
                    Object.Instantiate(_prefabs[(int)_testCase.PrimitiveShape], positions[i], rotations[i]);
                gameObject.transform.localScale =
                    new Vector3(_testCase.Scale, _testCase.Scale, _testCase.Scale);
            }
            
            generator.SetCameraPosition(_camera);
            
            _testManager.PublishMessage("Execution...");
            
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
            Physics.simulationMode = SimulationMode.FixedUpdate;
            
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
            
            Physics.simulationMode = SimulationMode.Script;
            
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

        public void Dispose()
        {
        }
    }
}