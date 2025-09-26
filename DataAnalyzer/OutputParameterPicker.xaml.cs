using System.ComponentModel;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DataAnalyzer;

public partial class OutputParameterPicker : WizardPage
{
    private readonly Aggregator _aggregator;

    private List<string> _parameters = [];

    public List<string> Parameters
    {
        get => _parameters;
        set => SetField(ref _parameters, value);
    }
    
    private string? _selectedParameter;

    public string? SelectedParameter
    {
        get => _selectedParameter;
        set
        {
            if (SetField(ref _selectedParameter, value))
            {
                CanGoNext = _selectedParameter != null;
                
                if (_selectedParameter != null)
                    _aggregator.OutputParameter = _selectedParameter;
            };
        }
    }


    public OutputParameterPicker(Aggregator aggregator)
    {
        _aggregator = aggregator;
        InitializeComponent();
        Update();
    }

    public sealed override void Update()
    {
        base.Update();
        var allTestRuns = TestRun.GetTestRuns(_aggregator.DataDirectory);
        var selectedTestRuns = new List<TestRun>();
        foreach (var testCase in _aggregator.TestCases)
        {
            selectedTestRuns.AddRange(allTestRuns.FindAll(x => x.Name == testCase.Name));
        }


        var allResults = selectedTestRuns.Select(tr => tr.TestResult.KeyValues).ToList();

        if (allResults.Count == 0) return;

        var commonParameters = allResults.Skip(1).Aggregate(new HashSet<string>(allResults.First().Keys),
            (acc, d) =>
            {
                acc.IntersectWith(d.Keys);
                return acc;
            });

        var uprofParameters = new List<string>();
        if (allTestRuns.First().TestResult.UprofData.Count != 0)
        {
            foreach (var parameter in allTestRuns.First().TestResult.UprofData.Keys)
            {
                uprofParameters.Add(parameter);
            }
        }
        
        Parameters = commonParameters.ToList().Concat(uprofParameters).ToList(); 
        SelectedParameter = Parameters.FirstOrDefault();
    }
}