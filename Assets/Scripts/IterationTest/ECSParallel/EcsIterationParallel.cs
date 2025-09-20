using Core.Main;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace IterationTest.ECSParallel
{
    [Preserve]
    public class EcsIterationParallel : IterationTestCase
    {
        public EcsIterationParallel()
        {
            
        }

        public EcsIterationParallel(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
        {
            
        }

        public EcsIterationParallel(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(tableRowTemplate,
            entry)
        {
            
        }
    }
}