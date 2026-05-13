using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using JADY.Core.Models;
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
            LaunchApp(serviceProvider, desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void LaunchApp(ServiceProvider serviceProvider, IClassicDesktopStyleApplicationLifetime desktop)
    {
        var saveService = serviceProvider.GetRequiredService<ISaveService>();
        
        if (!saveService.ExistsConfig())
        {
            // Show welcome window
            var welcomeWindow = desktop.MainWindow = serviceProvider.GetRequiredService<WelcomeWindow>();

            // Make sure app doesnt shutdown
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Show main window once welcome window is closed
            welcomeWindow.Closing += (_, _) =>
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                
                InitializeMainWindow(serviceProvider, desktop, saveService, out var mainWindow);
                
                mainWindow.Show();
            };
        }
        else
        {
            // Skip welcome window
            InitializeMainWindow(serviceProvider, desktop, saveService, out _);
        }
    }

    private static void InitializeMainWindow(ServiceProvider serviceProvider, IClassicDesktopStyleApplicationLifetime desktop, ISaveService saveService, out MainWindow mainWindow)
    {
        saveService.LoadConfig();
        saveService.LoadSave();
        
        desktop.MainWindow = mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
        };
    }

    private static void ConfigureServices(out ServiceProvider serviceProvider)
    {
        var collection = new ServiceCollection();

        collection.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // Services
        collection.AddSingleton<IEncryptionService, EncryptionService>();
        collection.AddSingleton<IAppVisualService, AppVisualService>();
        collection.AddSingleton<ISaveCoreService, SaveCoreService>();
        collection.AddSingleton<ISaveService, SaveService>();
        collection.AddSingleton<IWindowService, WindowService>();
        
        // Factories
        collection.AddSingleton<IDiaryEntryViewModelFactory, DiaryEntryViewModelFactory>();
        collection.AddSingleton<IDiaryViewModelFactory, DiaryViewModelFactory>();
        
        // View Models
        collection.AddSingleton<MainWindowViewModel>();
        
        // Views
        collection.AddTransient<SettingsWindow>();
        collection.AddTransient<AddEntryWindow>();
        collection.AddTransient<AddDiaryWindow>();
        collection.AddTransient<EditDiaryWindow>();
        collection.AddTransient<EditEntryWindow>();
        collection.AddTransient<EndEntryWindow>();
        collection.AddTransient<UnsavedChangesWindow>();
        collection.AddTransient<WelcomeWindow>();

        serviceProvider = collection.BuildServiceProvider();
    }
}