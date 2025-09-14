using System;
using System.Threading;
using Core;
using Core.Tests;
using Cysharp.Threading.Tasks;
using IterationTest;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace PhysicsTest.OOP
{
    public class TestLogic : IAsyncStartable, IDisposable
    {
        private readonly GameObject[] _prefabs;
        private readonly TestManager _testManager;
        private readonly OopPhysicsTestCase _testCase;
        
        public TestLogic(GameObject[] prefabs, TestManager testManager, DefaultParameters parameters)
        {
            _prefabs = prefabs;
            _testManager = testManager;
            _testCase = testManager.CurrentTestCase as OopPhysicsTestCase ?? new OopPhysicsTestCase
            {
                ArrangementShape = parameters.arrangementShape,
                PrimitiveShape = parameters.primitiveShape,
                Count = parameters.count,
                Scale = parameters.scale,
                HeightOffset = parameters.offset,
                PackingFactor = parameters.packingFactor
            };
        }
        
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            var generator = _testCase.ArrangementShape.GetGenerator(_testCase.Count, _testCase.PrimitiveShape, _testCase.Scale, _testCase.HeightOffset, _testCase.PackingFactor);
            var positions = generator.GetVectors();
            var rotations = generator.GetRotations();

            for (var i = 0; i < positions.Count; i++)
            {
                var gameObject = Object.Instantiate(_prefabs[(int)_testCase.PrimitiveShape], positions[i], rotations[i]);
                gameObject.transform.localScale =
                    new Vector3(_testCase.Scale, _testCase.Scale, _testCase.Scale);
            }
        }

        public void Dispose()
        {
            
        }
    }
}