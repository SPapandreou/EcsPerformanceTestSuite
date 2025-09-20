using System;
using System.IO;
using Core.Main;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Core.Tests
{
    public abstract class TestCase
    {
        private UniTaskCompletionSource _tcs = new();
        public string OutputDirectory { get; private set; }
        public bool ExitAfterExecution { get; set; }

        public bool Warmup { get; set; }

        private readonly VisualTreeAsset _tableRowTemplate;

        public TestCase()
        {
            
        }

        public TestCase(VisualTreeAsset tableRowTemplate)
        {
            _tableRowTemplate = tableRowTemplate;
        }

        public TestTableRow TestTableRow
        {
            get
            {
                _testTableRow ??= GetTestTableRow(_tableRowTemplate);
                return _testTableRow;
            }
        }
        
        public abstract TestRunFileEntry GetTestRunFileEntry();
        
        private TestTableRow _testTableRow;

        public void TestFinished()
        {
            _tcs.TrySetResult();

#if UNITY_EDITOR
            if (ExitAfterExecution)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }

        public async UniTask Run()
        {
            SceneManager.LoadScene(GetType().Name);

            await _tcs.Task;
            _tcs = new UniTaskCompletionSource();
        }

        protected abstract TestTableRow GetTestTableRow(VisualTreeAsset tableRowTemplate);

        public void CreateOutputDirectory(string resultDirectory)
        {
            OutputDirectory = $"{GetType().Name}_{DateTimeOffset.Now:MM-dd-yyyy_HH-mm-ss}";
            OutputDirectory = Path.Combine(resultDirectory, OutputDirectory);
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                TestTableRow.Root.AddToClassList("tableRowSelected");    
            }
            else
            {
                TestTableRow.Root.RemoveFromClassList("tableRowSelected");
            }
            
        }
    }
}