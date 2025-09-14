using Core.Tests;
using Core.uProf;
using VContainer;
using VContainer.Unity;

namespace IterationTest.ECS
{
    public class TestScope : LifetimeScope
    {
        public DefaultParameters defaultParameters;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<ITestCaseFactory<EcsIterationTestCase>>(defaultParameters);

            builder.Register<UprofWrapper>(Lifetime.Transient);

            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}