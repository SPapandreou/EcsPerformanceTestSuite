using Core.Tests;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Core.Main
{
    public class MainMenuScope : LifetimeScope
    {
        public MainMenuView mainMenuView;
        public VisualTreeAsset tableRowTemplate;

        protected override void Configure(IContainerBuilder builder)
        {
            var testCaseFactory = new TestCaseFactory(tableRowTemplate);
            builder.RegisterInstance(testCaseFactory);
            builder.RegisterComponent(mainMenuView);
            builder.Register<TestCaseList>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MainMenuLogic>();
        }
    }
}