using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JADY.UI.Views.Windows;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Services;

public class AppStartupService(IServiceProvider serviceProvider, ISaveService saveService, IEncryptionService encryptionService, IWindowService windowService) : IAppStartupService
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    
    public async Task AppStartup(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
        
        // Make sure app doesnt shutdown
        _desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        
        if (!saveService.ExistsConfig())
        {
            StartupWelcomeWindow();
        }
        else
        {
            await LoadSave();
            StartupMainWindow(out _);
        }
    }

    private void StartupWelcomeWindow()
    {
        // Show welcome window
        var welcomeWindow = _desktop.MainWindow = serviceProvider.GetRequiredService<WelcomeWindow>();
        
        welcomeWindow.Closing += WelcomeWindowOnClosing;
    }

    private async void WelcomeWindowOnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Show main window once welcome window is closed
        await LoadSave();
        StartupMainWindow(out var mainWindow);
                
        mainWindow.Show();
    }

    private async Task LoadSave()
    {
        _desktop.MainWindow = new LoadingSaveWindow();
        _desktop.MainWindow.Show();

        saveService.LoadConfig();
        await saveService.LoadSave();
        
        _desktop.MainWindow.Close();
    }

    private void StartupMainWindow(out MainWindow mainWindow)
    {
        _desktop.MainWindow = mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
        };
        _desktop.MainWindow.Show();
        
        // Set shutdown mode back to normal
        _desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
    }
}