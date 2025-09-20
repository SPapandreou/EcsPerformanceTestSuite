using System.ComponentModel;

namespace DataAnalyzer;

public partial class TestCasePicker : WizardPage
{
    
    private List<TestCase> _testCases = [];
    
    private readonly Aggregator _aggregator;

    public List<TestCase> TestCases
    {
        get => _testCases;
        set => SetField(ref _testCases, value);
    }
    
    public TestCasePicker(Aggregator aggregator)
    {
        _aggregator = aggregator;
        TestCases = TestCase.GetTestCases(aggregator.DataDirectory);
        foreach (var testCase in TestCases)
        {
            testCase.PropertyChanged += UpdateTestCaseList;
        }
        InitializeComponent();
        UpdateTestCaseList(null, null!);
    }

    private void UpdateTestCaseList(object? unusedSender, PropertyChangedEventArgs unusedPropertyChangedEventArgs)
    {
        _aggregator.TestCases = TestCases.Where(t => t.IsSelected).ToList();
        CanGoNext = TestCases.Any(testCase => testCase.IsSelected);
    }
}