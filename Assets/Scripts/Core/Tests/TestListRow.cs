using System;
using UnityEngine.UIElements;

namespace Core.Tests
{
    public class TestListRow
    {
        public VisualElement Root { get; }
        public Type Type { get; }
        
        public TestListRow(Type type)
        {
            Type = type;
            Root = new VisualElement();
            Root.Add(new Label(type.Name));
            Root.AddToClassList("testListRow");
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                Root.AddToClassList("testListRowSelected");
            }
            else
            {
                Root.RemoveFromClassList("testListRowSelected");
            }
        }

        public void RegisterClickCallback(EventCallback<ClickEvent> callback)
        {
            Root.RegisterCallback(callback);
        }
    }
}