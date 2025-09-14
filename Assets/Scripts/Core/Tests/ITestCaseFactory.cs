namespace Core.Tests
{
    public interface ITestCaseFactory<out T> where T : TestCase
    {
        T CreateTestCase();
    }
}