using Core.Tests;
using UnityEngine.UIElements;

namespace AnimationTest
{
    public class AnimationTestCase : TestCase
    {
        public int Count;
        public float Duration;
        
        protected override TestTableRow GetTestTableRow(UIDocument testTableRowTemplate)
        {
            var row = new TestTableRow(testTableRowTemplate);
            row.SetTestCase(GetType().Name);
            row.BindCountField(Count, x=>Count = x.newValue);
            row.BindDurationField(Duration, x=>Duration = x.newValue);
            return row;
        }
    }
}