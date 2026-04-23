using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JADY.ViewModels;
using JADY.Views;
using Microsoft.Extensions.DependencyInjection;

namespace JADY;

public partial class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureServices(out var serviceProvider);
        
        ServiceProvider = serviceProvider;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(out ServiceProvider serviceProvider)
    {
        var collection = new ServiceCollection();
        
        collection.AddSingleton<MainWindowViewModel>();

        serviceProvider = collection.BuildServiceProvider();
    }
}