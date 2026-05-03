using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JADY.Factories;
using JADY.Services;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AddDiaryWindow = JADY.Views.Dialogs.AddDiaryWindow;
using AddEntryWindow = JADY.Views.Dialogs.AddEntryWindow;
using EditDiaryWindow = JADY.Views.Dialogs.EditDiaryWindow;
using EditEntryWindow = JADY.Views.Dialogs.EditEntryWindow;
using EndEntryWindow = JADY.Views.Dialogs.EndEntryWindow;
using MainWindow = JADY.Views.Windows.MainWindow;
using SettingsWindow = JADY.Views.Dialogs.SettingsWindow;
using UnsavedChangesWindow = JADY.Views.Dialogs.UnsavedChangesWindow;

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
        collection.AddTransient<SettingsWindow>();
        collection.AddTransient<AddEntryWindow>();
        collection.AddTransient<AddDiaryWindow>();
        collection.AddTransient<EditDiaryWindow>();
        collection.AddTransient<EditEntryWindow>();
        collection.AddTransient<EndEntryWindow>();
        collection.AddTransient<UnsavedChangesWindow>();

        serviceProvider = collection.BuildServiceProvider();
    }
}