using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using JADY.Factories;
using JADY.Services;
using JADY.UI.Views.Windows;
using JADY.UI.Views.Dialogs;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "JADY.UI.Views.Controls")]

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
            var welcomeWindow = desktop.MainWindow = new WelcomeWindow()
            {
                DataContext = serviceProvider.GetRequiredService<WelcomeWindowViewModel>(),
            };

            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            welcomeWindow.Closing += (_, _) =>
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                
                var mainWindow = desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
                };
                
                mainWindow.Show();
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
        collection.AddTransient<WelcomeWindowViewModel>();
        
        // Views
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