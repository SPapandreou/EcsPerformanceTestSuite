using Core.Tests;
using Core.uProf;
using VContainer;
using VContainer.Unity;

namespace IterationTest.OOP
{
    public class TestScope : LifetimeScope
    {
        public DefaultParameters defaultParameters;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<UprofWrapper>(Lifetime.Transient);
            builder.RegisterInstance<ITestCaseFactory<OopIterationTestCase>>(defaultParameters);
            
            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}