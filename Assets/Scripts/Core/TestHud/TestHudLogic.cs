using System;
using Core.Tests;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using VContainer.Unity;
using R3;
using UnityEngine;

namespace Core.TestHud
{
    public class TestHudLogic : IStartable, IDisposable
    {
        private readonly TestHudView _testHudView;
        private readonly TestManager _testManager;
        
        private IDisposable _testManagerSubscription;
        private UniTaskCompletionSource _finishTcs;
        
        public TestHudLogic(TestHudView testHudView, TestManager testManager)
        {
            _testHudView = testHudView;
            _testManager = testManager;
        }
        
        public void Start()
        {
            _testManagerSubscription = _testManager.Messages.Subscribe(WriteMessage);
        }

        public void SetTitle(string title)
        {
            _testHudView.TitleLabel.text = title;
        }

        public void SetFpsEnabled(bool enabled)
        {
            _testHudView.FpsDisplay.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetFinishTestEnabled(bool enabled)
        {
            _testHudView.FinishTestButton.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
            if (!enabled) return;
            
            _finishTcs = new UniTaskCompletionSource();
            _testHudView.FinishTestButton.clicked += TestFinished;
        }

        private void TestFinished()
        {
            _finishTcs.TrySetResult();
        }

        public async UniTask WaitForFinish()
        {
            await _finishTcs.Task;
        }

        public void SetFps(double fps)
        {
            _testHudView.FpsLabel.text = fps.ToString("F2");
        }

        public void WriteMessage(string message)
        {
            var label = new Label(message);
            _testHudView.MessageView.Add(label);
            _testHudView.MessageView.schedule.Execute(() =>
            {
                var scrollView = _testHudView.MessageView;

                // Force scroll to bottom
                scrollView.scrollOffset = new Vector2(
                    0,
                    scrollView.contentContainer.worldBound.height - scrollView.contentViewport.worldBound.height
                );
            });
        }

        public void Dispose()
        {
            _testManagerSubscription.Dispose();
        }
        
    }
}