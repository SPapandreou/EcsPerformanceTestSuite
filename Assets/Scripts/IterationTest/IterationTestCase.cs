using Core.Tests;
using UnityEngine.UIElements;

namespace IterationTest
{
    public abstract class IterationTestCase : TestCase
    {
        public int Count;
        
        protected override TestTableRow GetTestTableRow(UIDocument tableRowTemplate)
        {
            var row = new TestTableRow(tableRowTemplate);
            row.SetTestCase(GetType().Name);
            row.BindCountField(Count, x=>Count = x.newValue);
            return row;
        }
    }
}