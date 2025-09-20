using Core.Main;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace IterationTest.ECSBurst
{
    [Preserve]
    public class EcsIterationBurst : IterationTestCase
    {
        public EcsIterationBurst()
        {
            
        }

        public EcsIterationBurst(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
        {
            
        }
        
        public EcsIterationBurst(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(
            tableRowTemplate, entry)
        {
            
        }
    }
}