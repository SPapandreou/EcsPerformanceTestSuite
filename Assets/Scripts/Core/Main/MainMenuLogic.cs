using System.Collections.Generic;
using System.Linq;
using Core.Configuration;
using Core.Tests;
using Cysharp.Threading.Tasks;
using SFB;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace Core.Main
{
    public class MainMenuLogic : IStartable
    {
        private readonly MainMenuView _mainMenuView;
        private readonly TestCaseFactory _testCaseFactory;
        private readonly TestManager _testManager;
        private readonly TestCaseList _testCaseList;
        private readonly AppConfig _appConfig;

        private TestListRow _selected;
        private List<TestListRow> _testListRows;

        public MainMenuLogic(MainMenuView mainMenuView, TestCaseFactory testCaseFactory, TestManager testManager, TestCaseList testCaseList, AppConfig config)
        {
            _mainMenuView = mainMenuView;
            _testCaseFactory = testCaseFactory;
            _testManager = testManager;
            _testCaseList = testCaseList;
            _appConfig = config;
        }

        public void Start()
        {
            _testListRows = TestCaseFactory.GetTestCases();
            foreach (var testCase in _testListRows)
            {
                _mainMenuView.TestList.Add(testCase.Root);
                testCase.RegisterClickCallback(_ => HandleClick(testCase));
            }

            _selected = _testListRows[0];
            _selected.SetSelected(true);

            _mainMenuView.AddButton.clicked += HandleAdd;
            _mainMenuView.StartButton.clicked += HandleStart;
            _mainMenuView.DeleteButton.clicked += HandleDelete;
            _mainMenuView.LoadFileButton.clicked += HandleLoadFile;
            _mainMenuView.SaveFileButton.clicked += HandleSaveFile;
            _mainMenuView.DeleteButton.SetEnabled(false);

            _mainMenuView.UprofToggle.value = _appConfig.UprofEnable;
            _mainMenuView.UprofToggle.RegisterValueChangedCallback(x =>
            {
                _appConfig.UprofEnable = x.newValue;
            });
        }

        private void HandleAdd()
        {
            _testCaseList.Add(_selected.Type);
        }

        private void HandleDelete()
        {
            _testCaseList.Delete();
        }

        private void HandleClick(TestListRow testCase)
        {
            _selected?.SetSelected(false);
            _selected = testCase;
            _selected.SetSelected(true);
        }

        private void HandleStart()
        {
            _testManager.Run().Forget();
        }

        private void HandleLoadFile()
        {
             var extensions = new[]
             {
                 new ExtensionFilter("Json Files", "json")
             };
            
            var paths = StandaloneFileBrowser.OpenFilePanel("Open test run file", _appConfig.TestRunFileDirectory, extensions, false);

             if (paths.Length == 0) return;
            
             var testRuns = TestRunFile.Load(paths.First());
             foreach (var testRun in testRuns.TestRuns)
             {
                 _testCaseList.Add(_testCaseFactory.CreateTestCase(testRun));
             }
        }

        private void HandleSaveFile()
        {
            var testRuns = new TestRunFile();
            
            foreach (var testRun in _testManager)
            {
                testRuns.Add(testRun);
            }
            
            var path = StandaloneFileBrowser.SaveFilePanel("Save test run file", _appConfig.TestRunFileDirectory, "",
                "json");
            
            if (string.IsNullOrEmpty(path)) return;
            
            testRuns.WriteToFile(path);
        }
    }
}