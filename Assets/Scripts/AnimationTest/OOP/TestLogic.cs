using System;
using System.Collections.Generic;
using System.Threading;
using Core.Statistics;
using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace AnimationTest.OOP
{
    public class TestLogic : IAsyncStartable, IDisposable
    {
        private readonly TestResults _testResults = new();
        private readonly PeterController _prefab;
        private readonly FpsCounter _fpsCounter;
        private readonly TestManager _testManager;
        private readonly Camera _mainCamera;
        private readonly CinemachinePositionComposer _positionComposer;

        private readonly List<GameObject> _objects = new();

        private readonly OopAnimation _testCase;
        
        private readonly TestHudLogic _testHudLogic;
        
        private readonly IUprofWrapper _uprofWrapper;

        private PeterController _centerPeter;

        public TestLogic(PeterController prefab, FpsCounter fpsCounter,
            TestManager testManager, Camera mainCamera, CinemachinePositionComposer positionComposer,
            ITestCaseFactory<OopAnimation> defaultFactory, TestHudLogic testHudLogic, IUprofWrapper uprofWrapper)
        {
            _prefab = prefab;
            _fpsCounter = fpsCounter;
            _testManager = testManager;
            _mainCamera = mainCamera;
            _positionComposer = positionComposer;

            _testCase = _testManager.GetOrCreateTestCase(defaultFactory);
            _testResults.TestCase = nameof(OopAnimation);
            _testResults.Parameters["Count"] = _testCase.Count;
            _testResults.Parameters["Duration"] = _testCase.Duration;
            
            _testHudLogic = testHudLogic;
            
            _uprofWrapper = uprofWrapper;
        }


        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            _testHudLogic.SetTitle(nameof(OopAnimation));
            
            _testManager.PublishMessage($"N = {_testCase.Count}");
            _testManager.PublishMessage($"t = {_testCase.Duration}");
            _testManager.PublishMessage("Setup...");

            await UniTask.Yield();

            var columns = Mathf.CeilToInt(Mathf.Sqrt(_testCase.Count));
            var rows = Mathf.CeilToInt((float)_testCase.Count / columns);

            var centerX = (columns - 1) / 2;
            var centerY = (rows - 1) / 2;

            for (var i = 0; i < _testCase.Count; i++)
            {
                var row = i / columns;
                var col = i % columns;

                var position = new Vector3(col, 0, row);

                var peter = Object.Instantiate(_prefab, position, Quaternion.identity);

                if (col == centerX && row == centerY)
                {
                    _centerPeter = peter;
                }

                _objects.Add(peter.gameObject);
            }

            _positionComposer.VirtualCamera.Follow = _centerPeter.transform;
            var xRotation = _positionComposer.VirtualCamera.transform.eulerAngles.x * Mathf.Deg2Rad;

            var fov = _mainCamera.fieldOfView * Mathf.Deg2Rad;
            var angle = xRotation + fov / 2;

            var halfDiagonal = rows / 2f * Mathf.Sqrt(2);


            var distance = halfDiagonal / Mathf.Sin(angle);
            distance *= 1.5f;

            _positionComposer.CameraDistance = distance;
            
            var lastCameraPosition = _positionComposer.VirtualCamera.transform.position;

            await UniTask.NextFrame();

            while (Vector3.SqrMagnitude(lastCameraPosition - _positionComposer.VirtualCamera.transform.position) >
                   0.001f)
            {
                lastCameraPosition = _positionComposer.VirtualCamera.transform.position;
                await UniTask.NextFrame();
            }

            foreach (var peter in _objects)
            {
                peter.GetComponent<PeterController>().StartTest();
            }
            
            _testManager.PublishMessage("Execution...");
            
            await UniTask.NextFrame();

            await _uprofWrapper.StartProfiling();
            
            _fpsCounter.Start();
            while (_fpsCounter.TotalTime < _testCase.Duration)
            {
                await UniTask.NextFrame();
            }

            _fpsCounter.Stop();
            
            _testManager.PublishMessage("Cleanup...");
            
            await UniTask.Yield();
            
            foreach (var peter in _objects)
            {
                peter.GetComponent<PeterController>().StopTest();
            }

            await _uprofWrapper.StopProfiling(_testCase.OutputDirectory, _testResults);

            await UniTask.Yield();

            foreach (var obj in _objects)
            {
                Object.Destroy(obj);
            }

            _objects.Clear();

            _testResults.KeyValues["AverageFps"] = _fpsCounter.AverageFps;
            _testResults.KeyValues["MinFps"] = _fpsCounter.MinFps;
            _testResults.KeyValues["MaxFps"] = _fpsCounter.MaxFps;
            _testResults.TimeSeriesData["Fps"] = _fpsCounter.GetFpsTimeSeries();

            if (!_testCase.Warmup)
            {
                _testResults.WriteToFile(_testCase.OutputDirectory);    
            }
            _testCase.TestFinished();
        }

        public void Dispose()
        {
            _fpsCounter.Dispose();
        }
    }
}