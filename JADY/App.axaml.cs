using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JADY.Factories;
using JADY.Services;
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
        
        // Services
        collection.AddSingleton<IAppVisualService, AppVisualService>();
        collection.AddSingleton<ISaveService, SaveService>();
        
        // Factories
        collection.AddSingleton<IDiaryEntryViewModelFactory, DiaryEntryViewModelFactory>();
        
        // View Models
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddTransient<SettingsWindow>();
        collection.AddTransient<AddEntryWindow>();
        collection.AddTransient<AddDiaryWindow>();
        collection.AddTransient<EditDiaryWindow>();
        collection.AddTransient<EditEntryWindow>();
        collection.AddTransient<EndEntryWindow>();

        serviceProvider = collection.BuildServiceProvider();
    }
}