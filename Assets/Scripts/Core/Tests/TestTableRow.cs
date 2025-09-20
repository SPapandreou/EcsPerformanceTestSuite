using System;
using Core.Shapes;
using UnityEngine.UIElements;

namespace Core.Tests
{
    public class TestTableRow
    {
        private readonly Label _testCaseLabel;
        private readonly IntegerField _countField;
        private readonly FloatField _durationField;
        private readonly EnumField _arrangementField;
        private readonly EnumField _shapeField;
        private readonly FloatField _packingField;
        private readonly FloatField _scaleField;
        private readonly FloatField _heightOffsetField;
        
        
        public VisualElement Root { get; }
        
        public TestTableRow(VisualTreeAsset template)
        {
            Root = template.CloneTree();
            
            _testCaseLabel = Root.Q<Label>("TestCaseLabel");
            _countField = Root.Q<IntegerField>("CountField");
            _durationField = Root.Q<FloatField>("DurationField");
            _arrangementField = Root.Q<EnumField>("ArrangementField");
            _shapeField = Root.Q<EnumField>("ShapeField");
            _packingField = Root.Q<FloatField>("PackingField");
            _scaleField = Root.Q<FloatField>("ScaleField");
            _heightOffsetField = Root.Q<FloatField>("HeightOffsetField");

            _countField.SetEnabled(false);
            _durationField.SetEnabled(false);
            _arrangementField.SetEnabled(false);
            _shapeField.SetEnabled(false);
            _packingField.SetEnabled(false);
            _scaleField.SetEnabled(false);
            _heightOffsetField.SetEnabled(false);
        }

        public void SetTestCase(string testCase)
        {
            _testCaseLabel.text = testCase;
        }

        public void BindCountField(int value, EventCallback<ChangeEvent<int>> setCount)
        {
            _countField.SetEnabled(true);
            _countField.value = value;
            _countField.RegisterValueChangedCallback(setCount);
        }

        public void BindDurationField(float value, EventCallback<ChangeEvent<float>> setDuration)
        {
            _durationField.SetEnabled(true);
            _durationField.value = value;
            _durationField.RegisterValueChangedCallback(setDuration);
        }

        public void BindArrangementField(ArrangementShape value, EventCallback<ChangeEvent<Enum>> setArrangement)
        {
            _arrangementField.SetEnabled(true);
            _arrangementField.value = value;
            _arrangementField.RegisterValueChangedCallback(setArrangement);
        }

        public void BindShapeField(PrimitiveShape value, EventCallback<ChangeEvent<Enum>> setShape)
        {
            _shapeField.SetEnabled(true);
            _shapeField.value = value;
            _shapeField.RegisterValueChangedCallback(setShape);
        }

        public void BindPackingField(float value, EventCallback<ChangeEvent<float>> setPacking)
        {
            _packingField.SetEnabled(true);
            _packingField.value = value;
            _packingField.RegisterValueChangedCallback(setPacking);
        }

        public void BindScaleField(float value, EventCallback<ChangeEvent<float>> setScale)
        {
            _scaleField.SetEnabled(true);
            _scaleField.value = value;
            _scaleField.RegisterValueChangedCallback(setScale);
        }

        public void BindHeightOffsetField(float value, EventCallback<ChangeEvent<float>> setHeightOffset)
        {
            _heightOffsetField.SetEnabled(true);
            _heightOffsetField.value = value;
            _heightOffsetField.RegisterValueChangedCallback(setHeightOffset);
        }
    }
}