using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DataAnalyzer;

public class WizardPage : UserControl, INotifyPropertyChanged
{
    private bool _canGoNext = true;

    public bool CanGoNext
    {
        get => _canGoNext;
        set => SetField(ref _canGoNext, value);
    }

    public virtual void Update()
    {
        
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