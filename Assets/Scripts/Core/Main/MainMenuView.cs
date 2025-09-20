using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Main
{
    public class MainMenuView : MonoBehaviour
    {
        public UIDocument mainMenu;

        public VisualElement TestList;
        public Button AddButton;
        public Button StartButton;
        public Button DeleteButton;
        public Button LoadFileButton;
        public Button SaveFileButton;
        public ScrollView TestCasesView;
        public Toggle UprofToggle;
        
        private void Awake()
        {
            TestList = mainMenu.rootVisualElement.Q<VisualElement>("TestList");
            AddButton = mainMenu.rootVisualElement.Q<Button>("AddButton");
            StartButton = mainMenu.rootVisualElement.Q<Button>("StartButton");
            DeleteButton = mainMenu.rootVisualElement.Q<Button>("DeleteButton");
            TestCasesView = mainMenu.rootVisualElement.Q<ScrollView>("TestCasesView");
            UprofToggle = mainMenu.rootVisualElement.Q<Toggle>("UprofToggle");
            LoadFileButton = mainMenu.rootVisualElement.Q<Button>("LoadFileButton");
            SaveFileButton = mainMenu.rootVisualElement.Q<Button>("SaveFileButton");
        }
    }
}