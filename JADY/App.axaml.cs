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
            SetupMainWindow(serviceProvider, desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void SetupMainWindow(ServiceProvider serviceProvider, IClassicDesktopStyleApplicationLifetime desktop)
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
                
                var mainWindow = desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
                };
                
                mainWindow.Show();
            };
        }
        else
        {
            // Skip welcome window
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }
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