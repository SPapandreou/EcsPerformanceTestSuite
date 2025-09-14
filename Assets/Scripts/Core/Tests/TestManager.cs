using System.Collections.Generic;
using Core.Configuration;
using Core.Main;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.SceneManagement;

namespace Core.Tests
{
    public class TestManager
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
            foreach (var testCase in _testCases)
            {
                CurrentTestCase = testCase;
                await CurrentTestCase.Run();
            }

            SceneManager.LoadScene(nameof(MainMenu));
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
            testCase.CreateOutputDirectory(_config.ResultDirectory);
            CurrentTestCase = testCase;

            return testCase;
        }
    }
}