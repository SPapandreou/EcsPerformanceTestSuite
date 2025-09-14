using Core.Configuration;
using Core.Tests;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public class RootScope : LifetimeScope
    {
        public UIDocument testTableRowTemplate;
        
        protected override void Configure(IContainerBuilder builder)
        {
#if UNITY_EDITOR
            var appConfig = new AppConfig
            {
                UprofWrapperPath = "C:\\GameDevelopment\\UnityProjects\\EcsPerformanceTestSuite\\UprofWrapper\\bin\\Release\\net9.0\\UprofWrapper.exe",
                UprofBinaryPath = "C:\\Program Files\\AMD\\AMDuProf\\bin\\AMDuProfCLI.exe",
                UprofTemp = "C:\\Users\\Simon\\Desktop\\uprofTemp",
                ResultDirectory = "C:\\Users\\Simon\\Desktop\\testResults",
                UprofEnable = true
            };
            builder.RegisterInstance(appConfig);
#endif
            builder.Register<TestManager>(Lifetime.Singleton);
            builder.Register<FpsCounter>(Lifetime.Transient);
        }
    }
}