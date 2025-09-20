using Core.TestHud;
using Core.Tests;
using Core.uProf;
using Core.Uprof;
using VContainer;
using VContainer.Unity;

namespace IterationTest.ECSParallel
{
    public class TestScope : LifetimeScope
    {
        public DefaultParameters defaultParameters;
        public TestHudView testHudView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<ITestCaseFactory<EcsIterationParallel>>(defaultParameters);
            builder.RegisterComponent(testHudView);

            builder.Register<IUprofWrapper, NativeUprofWrapper>(Lifetime.Transient);

            builder.RegisterEntryPoint<TestHudLogic>().AsSelf();
            builder.RegisterEntryPoint<TestLogic>();
        }
    }
}