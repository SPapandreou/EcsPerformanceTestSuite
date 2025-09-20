using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

#nullable disable

namespace DataAnalyzer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;
    
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var serviceCollection = new ServiceCollection();
        
        ConfigureServices(serviceCollection);
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
        
        var dialog = new OpenFolderDialog
        {
            Title = "Select data folder"
        };

        if (dialog.ShowDialog() == true)
        {
            var aggregator = _serviceProvider.GetRequiredService<Aggregator>();
            aggregator.DataDirectory = dialog.FolderName;
        }
        else
        {
            Console.WriteLine("Error, no folder selected!");
            Application.Current.Shutdown(1);
        }
        
        var window = _serviceProvider.GetRequiredService<WizardShell>();
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        _serviceProvider.Dispose();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Aggregator>();
        services.AddSingleton<WizardShell>();
        services.AddSingleton<WizardPage, TestCasePicker>();
        services.AddSingleton<WizardPage, OutputParameterPicker>();
    }
}