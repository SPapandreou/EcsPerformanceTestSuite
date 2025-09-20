using Core.Main;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace AnimationTest.ECS
{
    [Preserve]
    public class EcsAnimation : AnimationTestCase
    {
        public EcsAnimation()
        {
            
        }

        public EcsAnimation(VisualTreeAsset tableRowTemplate) : base(tableRowTemplate)
        {
            
        }
        
        public EcsAnimation(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(
            tableRowTemplate, entry)
        {
            
        }   
    }
}