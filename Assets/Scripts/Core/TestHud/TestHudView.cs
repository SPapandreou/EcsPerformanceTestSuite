using UnityEngine;
using UnityEngine.UIElements;

namespace Core.TestHud
{
    public class TestHudView : MonoBehaviour
    {
        public UIDocument hud;

        public Label TitleLabel;
        public Label FpsLabel;
        public VisualElement FpsDisplay;
        public ScrollView MessageView;
        public Button FinishTestButton;

        private void Awake()
        {
            TitleLabel = hud.rootVisualElement.Q<Label>("TitleLabel");
            FpsLabel = hud.rootVisualElement.Q<Label>("FpsLabel");
            FpsDisplay = hud.rootVisualElement.Q<VisualElement>("FpsDisplay");
            MessageView = hud.rootVisualElement.Q<ScrollView>("MessageView");
            FinishTestButton = hud.rootVisualElement.Q<Button>("FinishTestButton");
            
            FinishTestButton.style.display = DisplayStyle.None;
            FpsDisplay.style.display = DisplayStyle.None;
        }
    }
}