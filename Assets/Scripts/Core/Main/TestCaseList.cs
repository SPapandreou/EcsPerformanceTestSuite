using System;
using Core.Tests;
using UnityEngine.UIElements;

namespace Core.Main
{
    public class TestCaseList
    {
        private readonly TestManager _testManager;
        private readonly TestCaseFactory _testCaseFactory;
        private readonly MainMenuView _mainMenuView;

        private TestCase _selected;

        private TestCase Selected
        {
            get => _selected;
            set
            {
                _selected?.SetSelected(false);
                
                if (_selected == value || value == null)
                {
                    _selected = null;
                    _mainMenuView.DeleteButton.SetEnabled(false);
                    return;
                }

                _selected = value;
                _selected.SetSelected(true);
                _mainMenuView.DeleteButton.SetEnabled(true);
            }
        }


        public TestCaseList(MainMenuView mainMenuView, TestManager testManager, TestCaseFactory testCaseFactory)
        {
            _testManager = testManager;
            _testCaseFactory = testCaseFactory;
            _mainMenuView = mainMenuView;
        }

        public void Add(Type testCaseType)
        {
            var testCase = _testCaseFactory.CreateTestCase(testCaseType);
            Add(testCase);
        }

        public void Add(TestCase testCase)
        {
            var row = testCase.TestTableRow;
            row.Root.RegisterCallback<ClickEvent>(_ =>
            {
                Selected = testCase;
            });
            _testManager.AddTestCase(testCase);
            _mainMenuView.TestCasesView.Add(row.Root);
        }

        public void Delete()
        {
            if (Selected == null) return;
            _testManager.RemoveTestCase(Selected);
            _mainMenuView.TestCasesView.Remove(Selected.TestTableRow.Root);
            Selected = null;
        }
    }
}