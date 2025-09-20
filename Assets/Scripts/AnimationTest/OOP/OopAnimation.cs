using Core.Main;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace AnimationTest.OOP
{
    [Preserve]
    public class OopAnimation : AnimationTestCase
    {
        public OopAnimation()
        {
            
        }
        public OopAnimation(VisualTreeAsset testTableRowTemplate) : base(testTableRowTemplate)
        {
        }
        
        public OopAnimation(VisualTreeAsset tableRowTemplate, TestRunFileEntry entry) : base(
            tableRowTemplate, entry)
        {
            
        }
    }
}