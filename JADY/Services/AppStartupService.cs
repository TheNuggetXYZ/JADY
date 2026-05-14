using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JADY.UI.Views.Windows;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Services;

public class AppStartupService(IServiceProvider serviceProvider, ISaveService saveService) : IAppStartupService
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    
    public void AppStartup(IClassicDesktopStyleApplicationLifetime desktop)
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
            // TODO: login
            
            // Skip welcome window
            StartupMainWindow(out _);
        }
    }

    private void StartupWelcomeWindow()
    {
        // Show welcome window
        var welcomeWindow = _desktop.MainWindow = serviceProvider.GetRequiredService<WelcomeWindow>();
        
        welcomeWindow.Closing += WelcomeWindowOnClosing;
    }

    private void WelcomeWindowOnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Show main window once welcome window is closed
        StartupMainWindow(out var mainWindow);
                
        mainWindow.Show();
    }

    private void StartupMainWindow(out MainWindow mainWindow)
    {
        // Set shutdown mode back to normal
        _desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        
        saveService.LoadConfig();
        saveService.LoadSave();
        
        _desktop.MainWindow = mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
        };
    }
}