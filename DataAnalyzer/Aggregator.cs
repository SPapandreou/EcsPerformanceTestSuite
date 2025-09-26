using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32;

namespace DataAnalyzer;

public class Aggregator : INotifyPropertyChanged
{
    private string _dataDirectory = string.Empty;
    public string DataDirectory
    {
        get => _dataDirectory;
        set => SetField(ref _dataDirectory, value);
    }

    private List<TestCase> _testCases = new ();
    public List<TestCase> TestCases
    {
        get => _testCases;
        set
        {
            if (SetField(ref _testCases, value))
            {
                var testRuns = new List<TestRun>();
                var allTestRuns = TestRun.GetTestRuns(DataDirectory);
                foreach (var testCase in TestCases)
                {
                    testRuns.AddRange(allTestRuns.FindAll(x => x.Name == testCase.Name));
                }

                TestRuns = testRuns;
            };
        }
    }

    private List<TestRun> _testRuns = new ();

    public List<TestRun> TestRuns
    {
        get => _testRuns;
        set => SetField(ref _testRuns, value);
    }

    private string _outputParameter = string.Empty;
    
    public string OutputParameter
    {
        get => _outputParameter;
        set => SetField(ref _outputParameter, value);
    }

    public void Run()
    {
        Console.WriteLine("Run");
        
        var dialog = new SaveFileDialog
        {
            AddExtension = true,
            DefaultExt = "dat",
            Filter = "dat files (*.dat)|*.dat",
            Title = "Save aggregated dat file"
        };
        
        if (dialog.ShowDialog() == false)
        {
            return;
        }
        
        var outputFile = dialog.FileName;
        var selectedRuns = new List<TestRun>();

        foreach (var testCase in TestCases)
        {
            selectedRuns.AddRange(TestRuns.FindAll(x => x.Name == testCase.Name));
        }

        var allN = selectedRuns.Select(tr => Convert.ToInt32(tr.TestResult.Parameters["Count"])).Distinct().OrderBy(n => n)
            .ToList();
        
        var aggregated = new Dictionary<string, Dictionary<int, double>>();

        foreach (var testCase in TestCases)
        {
            var runsByN = selectedRuns.Where(tr => tr.Name == testCase.Name).GroupBy(tr => Convert.ToInt32(tr.TestResult.Parameters["Count"])).ToDictionary(g=>g.Key, g=> g.Select(r =>
            {
                if (r.TestResult.KeyValues.TryGetValue(OutputParameter, out var value))
                {
                    return value;
                    
                }

                if (r.TestResult.UprofData.TryGetValue(OutputParameter, out var uprofValue))
                {
                    return uprofValue;
                }

                throw new Exception($"Test run is missing parameter {OutputParameter}");
            }).ToList());

            var runCounts = runsByN.Values.Select(v => v.Count).Distinct().ToList();
            if (runCounts.Count > 1)
            {
                throw new Exception($"Incomplete test set for test case {testCase.Name}");
            }

            aggregated[testCase.Name] = runsByN.ToDictionary(kv => kv.Key, kv => kv.Value.Average());
        }

        var sb = new StringBuilder();

        sb.AppendLine($"N {string.Join(" ", TestCases.Select(t => t.Name))}");

        foreach (var n in allN)
        {
            var row = new List<string> { n.ToString() };
            foreach (var testCase in TestCases.Select(t => t.Name))
            {
                if (!aggregated[testCase].TryGetValue(n, out var avg))
                    throw new Exception($"Missing N={n} for test case {testCase}");
                row.Add(avg.ToString(CultureInfo.CurrentCulture));
            }
            
            sb.AppendLine(string.Join(" ", row));
        }
        
        File.WriteAllText(outputFile, sb.ToString());
        
        
        foreach (var test in _testCases)
        {
            Console.WriteLine(test.Name);
        }
        Console.WriteLine(OutputParameter);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}