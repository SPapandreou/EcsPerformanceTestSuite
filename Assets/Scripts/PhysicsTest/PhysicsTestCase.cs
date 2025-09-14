using Core;
using Core.Tests;
using UnityEngine.UIElements;

namespace PhysicsTest
{
    public abstract class PhysicsTestCase : TestCase
    {
        public int Count;
        public PrimitiveShape PrimitiveShape;
        public ArrangementShape ArrangementShape;
        public float Scale;
        public float HeightOffset;
        public float PackingFactor;
        
        protected override TestTableRow GetTestTableRow(UIDocument testTableRowTemplate)
        {
            var row = new TestTableRow(testTableRowTemplate);
            row.SetTestCase(GetType().Name);
            row.BindCountField(Count, x=>Count = x.newValue);
            row.BindShapeField(PrimitiveShape, x => PrimitiveShape = (PrimitiveShape)x.newValue);
            row.BindArrangementField(ArrangementShape, x=>ArrangementShape = (ArrangementShape)x.newValue);
            row.BindScaleField(Scale, x=>Scale = x.newValue);
            row.BindHeightOffsetField(HeightOffset, x=>HeightOffset = x.newValue);
            row.BindPackingField(PackingFactor, x=>PackingFactor = x.newValue);
            
            return row;
        }
    }
}