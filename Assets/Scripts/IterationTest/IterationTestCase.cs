using System;
using Core.Main;
using Core.Tests;
using UnityEngine.UIElements;

namespace IterationTest
{
    public abstract class IterationTestCase : TestCase
    {
        public int Count = 10;
        public int Iterations = 1;

        public IterationTestCase()
        {
            
        }

        public IterationTestCase(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(tableRowTemplate)
        {
            Count = Convert.ToInt32(entry.Parameters["Count"]);
            Iterations = Convert.ToInt32(entry.Parameters["Iterations"]);
        }

        public IterationTestCase(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
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
                    ["Iterations"] = Iterations
                }
            };

            return entry;
        }
    

        protected override TestTableRow GetTestTableRow(VisualTreeAsset tableRowTemplate)
        {
            var row = new TestTableRow(tableRowTemplate);
            row.SetTestCase(GetType().Name);
            row.BindCountField(Count, x=>Count = x.newValue);
            row.BindIterationsField(Iterations, x => Iterations = x.newValue);
            return row;
        }
    }
}