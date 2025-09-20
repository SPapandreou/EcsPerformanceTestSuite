using System;
using Core;
using Core.Main;
using Core.Shapes;
using Core.Tests;
using UnityEngine.UIElements;

namespace PhysicsTest
{
    public abstract class PhysicsTestCase : TestCase
    {
        public int Count = 10;
        public PrimitiveShape PrimitiveShape = PrimitiveShape.Sphere;
        public ArrangementShape ArrangementShape = ArrangementShape.Cube;
        public float Scale = 1f;
        public float HeightOffset = 1f;
        public float PackingFactor = 1f;

        public PhysicsTestCase()
        {
            
        }

        public PhysicsTestCase(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(tableRowTemplate)
        {
            Count = Convert.ToInt32(entry.Parameters["Count"]);
            PrimitiveShape = (PrimitiveShape)Enum.Parse(typeof(PrimitiveShape), (string)entry.Parameters["PrimitiveShape"]);
            ArrangementShape = (ArrangementShape)Enum.Parse(typeof(ArrangementShape), (string)entry.Parameters["ArrangementShape"]);
            Scale = Convert.ToSingle(entry.Parameters["Scale"]);
            HeightOffset = Convert.ToSingle(entry.Parameters["HeightOffset"]);
            PackingFactor = Convert.ToSingle(entry.Parameters["PackingFactor"]);
        }
        
        public PhysicsTestCase(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
        {
            
        }
        
        protected override TestTableRow GetTestTableRow(VisualTreeAsset testTableRowTemplate)
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
        
        public override TestRunFileEntry GetTestRunFileEntry()
        {
            var entry = new TestRunFileEntry
            {
                TestCase = GetType().Name,
                Parameters =
                {
                    ["Count"] = Count,
                    ["PrimitiveShape"] = PrimitiveShape.ToString(),
                    ["ArrangementShape"] = ArrangementShape.ToString(),
                    ["Scale"] = Scale,
                    ["HeightOffset"] = HeightOffset,
                    ["PackingFactor"] = PackingFactor
                }
            };

            return entry;
        }
    }
}