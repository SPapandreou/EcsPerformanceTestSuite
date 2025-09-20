using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DataAnalyzer;

public class TestRun : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    private string _dataPath = string.Empty;

    public string DataPath
    {
        get => _dataPath;
        set => SetField(ref _dataPath, value);
    }


    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }
    
    private TestResults _testResult = new();

    public TestResults TestResult
    {
        get => _testResult;
        set => SetField(ref _testResult, value);
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

    private void LoadResult()
    {
        TestResult = TestResults.ReadFromFolder(DataPath);
    }

    public static List<TestRun> GetTestRuns(string dataDir)
    {
        var result = new List<TestRun>();
        
        var directories = Directory.GetDirectories(dataDir);
        
        foreach (var dir in directories)
        {
            var testRun = new TestRun
            {
                DataPath = dir,
                Name = Path.GetFileName(dir).Split('_')[0]
            };
            testRun.LoadResult();
            result.Add(testRun);
        }

        return result;

    }
}