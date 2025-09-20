using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using VContainer;
using VContainer.Unity;

namespace IterationTest.OOP
{
    public class TestScope : LifetimeScope
    {
        public DefaultParameters defaultParameters;
        public TestHudView testHudView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Transient);
            builder.RegisterInstance<ITestCaseFactory<OopIteration>>(defaultParameters);
            builder.RegisterComponent(testHudView);
            
            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();
            
            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}