using System;
using System.Collections;
using System.Collections.Generic;
using Core.Configuration;
using Core.Main;
using Cysharp.Threading.Tasks;
using IterationTest.ECSBurst;
using IterationTest.ECSMainThread;
using IterationTest.ECSMathBurst;
using IterationTest.ECSMathParallel;
using IterationTest.ECSParallel;
using IterationTest.OOP;
using R3;
using UnityEngine.SceneManagement;

namespace Core.Tests
{
    public class TestManager : IEnumerable<TestCase>
    {
        public Observable<string> Messages => _messages;
        private readonly Subject<string> _messages = new();
        
        public TestCase CurrentTestCase { get; private set; }

        private readonly List<TestCase> _testCases = new();
        private readonly AppConfig _config;

        public TestManager(AppConfig config)
        {
            _config = config;
        }
        
        public async UniTask Run()
        {
            await Warmup();
            
            foreach (var testCase in _testCases)
            {
                CurrentTestCase = testCase;
                await CurrentTestCase.Run();
            }
            
            _testCases.Clear();
            CurrentTestCase = null;

            SceneManager.LoadScene(nameof(MainMenu));
        }

        private async UniTask Warmup()
        {
            var uProf = _config.UprofEnable;
            _config.UprofEnable = false;

            var testTypes = TestCaseFactory.GetTestCaseTypes();

            foreach (var testType in testTypes)
            {
                CurrentTestCase = (TestCase)Activator.CreateInstance(testType);
                CurrentTestCase.Warmup = true;
                await CurrentTestCase.Run();
            }
            
            _config.UprofEnable = uProf;
        }

        public void AddTestCase(TestCase testCase)
        {
            _testCases.Add(testCase);
        }

        public void RemoveTestCase(TestCase testCase)
        {
            _testCases.Remove(testCase);
        }

        public void PublishMessage(string message)
        {
            _messages.OnNext(message);
        }

        public T GetOrCreateTestCase<T>(ITestCaseFactory<T> defaultFactory) where T : TestCase
        {
            if (CurrentTestCase is not T testCase)
            {
                testCase = defaultFactory.CreateTestCase();
                testCase.ExitAfterExecution = true;
            }

            if (!testCase.Warmup)
            {
                testCase.CreateOutputDirectory(_config.ResultDirectory);    
            }
            CurrentTestCase = testCase;

            return testCase;
        }

        public IEnumerator<TestCase> GetEnumerator()
        {
            return _testCases.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}