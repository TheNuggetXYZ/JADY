using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using JADY.Factories;
using JADY.Services;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "JADY.Views.Controls")]

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
            desktop.MainWindow = new UI.Views.Windows.MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(out ServiceProvider serviceProvider)
    {
        var collection = new ServiceCollection();

        collection.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // Services
        collection.AddSingleton<IAppVisualService, AppVisualService>();
        collection.AddSingleton<ISaveCoreService, SaveCoreService>();
        collection.AddSingleton<ISaveService, SaveService>();
        collection.AddSingleton<IWindowService, WindowService>();
        
        // Factories
        collection.AddSingleton<IDiaryEntryViewModelFactory, DiaryEntryViewModelFactory>();
        collection.AddSingleton<IDiaryViewModelFactory, DiaryViewModelFactory>();
        
        // View Models
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddTransient<UI.Views.Dialogs.SettingsWindow>();
        collection.AddTransient<UI.Views.Dialogs.AddEntryWindow>();
        collection.AddTransient<UI.Views.Dialogs.AddDiaryWindow>();
        collection.AddTransient<UI.Views.Dialogs.EditDiaryWindow>();
        collection.AddTransient<UI.Views.Dialogs.EditEntryWindow>();
        collection.AddTransient<UI.Views.Dialogs.EndEntryWindow>();
        collection.AddTransient<UI.Views.Dialogs.UnsavedChangesWindow>();

        serviceProvider = collection.BuildServiceProvider();
    }
}