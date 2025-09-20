using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DataAnalyzer;

public partial class WizardShell : Window, INotifyPropertyChanged
{
    private WizardPage _currentPage;

    private string _nextLabel = "";
    private bool _backEnabled;

    private readonly Stack<WizardPage> _nextPages = new();
    private readonly Stack<WizardPage> _previousPages = new();
    
    private readonly Aggregator _aggregator;

    public string NextLabel
    {
        get => _nextLabel;
        set => SetField(ref _nextLabel, value);
    }

    public bool BackEnabled
    {
        get => _backEnabled;
        set => SetField(ref _backEnabled, value);
    }

    public WizardPage CurrentPage
    {
        get => _currentPage;
        set => SetField(ref _currentPage, value);
    }

    public WizardShell(Aggregator aggregator, IEnumerable<WizardPage> pages)
    {
        _aggregator = aggregator;
        _currentPage = new WizardPage();
        InitializeComponent();

        foreach (var page in pages.Reverse())
        {
            _nextPages.Push(page);
        }

        CurrentPage = _nextPages.Pop();
        UpdateControls();
    }

    private void UpdateControls()
    {
        NextLabel = _nextPages.Count == 0 ? "Finish" : "Next";
        BackEnabled = _previousPages.Count > 0;
    }

    private void Next()
    {
        if (_nextPages.Count == 0)
        {
            _aggregator.Run();
            Application.Current.Shutdown();
            return;
        }
        
        _previousPages.Push(CurrentPage);
        CurrentPage = _nextPages.Pop();
        CurrentPage.Update();
        UpdateControls();
    }

    private void Back()
    {
        _nextPages.Push(CurrentPage);
        CurrentPage = _previousPages.Pop();
        CurrentPage.Update();
        UpdateControls();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
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

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        Back();
    }

    private void NextButton_OnClick(object sender, RoutedEventArgs e)
    {
        Next();
    }
}