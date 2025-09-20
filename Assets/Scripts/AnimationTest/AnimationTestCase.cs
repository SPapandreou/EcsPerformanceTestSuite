using System;
using Core.Main;
using Core.Tests;
using UnityEngine.UIElements;

namespace AnimationTest
{
    public abstract class AnimationTestCase : TestCase
    {
        public int Count = 10;
        public float Duration = 1;

        public AnimationTestCase()
        {
            
        }

        public AnimationTestCase(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(tableRowTemplate)
        {
            Count = Convert.ToInt32(entry.Parameters["Count"]);
            Duration = Convert.ToSingle(entry.Parameters["Duration"]);
        }
        
        public AnimationTestCase(VisualTreeAsset testTableRowTemplate) : base(testTableRowTemplate)
        {
        }
        
        public override TestRunFileEntry GetTestRunFileEntry()
        {
            var entry = new TestRunFileEntry
            {
                TestCase = GetType().Name,
                Parameters =
                {
                    ["Count"] = Count,
                    ["Duration"] = Duration
                }
            };

            return entry;
        }
        
        protected override TestTableRow GetTestTableRow(VisualTreeAsset testTableRowTemplate)
        {
            var row = new TestTableRow(testTableRowTemplate);
            row.SetTestCase(GetType().Name);
            row.BindCountField(Count, x=>Count = x.newValue);
            row.BindDurationField(Duration, x=>Duration = x.newValue);
            return row;
        }
    }
}