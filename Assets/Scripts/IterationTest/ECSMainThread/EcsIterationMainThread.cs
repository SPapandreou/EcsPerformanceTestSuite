using Core.Main;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace IterationTest.ECSMainThread
{
    [Preserve]
    public class EcsIterationMainThread : IterationTestCase
    {
        public EcsIterationMainThread()
        {
            
        }

        public EcsIterationMainThread(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
        {
            
        }
        
        public EcsIterationMainThread(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(
            tableRowTemplate, entry)
        {
            
        }
    }
}