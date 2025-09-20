using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DataAnalyzer;

public class TestCase : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
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

    public static List<TestCase> GetTestCases(string dataDir)
    {
        var result = new List<TestCase>();
        var testCases = Directory.GetDirectories(dataDir)
            .Select(Path.GetFileName)
            .Select(d => d!.Split('_')[0])
            .Distinct();
        foreach (var testCaseName in testCases)
        {
            var testCase = new TestCase
            {
                Name = testCaseName
            };
            result.Add(testCase);
        }

        return result;
    }
}