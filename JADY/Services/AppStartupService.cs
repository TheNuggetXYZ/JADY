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
            // Show welcome window
            var welcomeWindow = _desktop.MainWindow = serviceProvider.GetRequiredService<WelcomeWindow>();
        
            var tcs = new TaskCompletionSource();
            welcomeWindow.Closed += (_, _) => tcs.SetResult();
            welcomeWindow.Show();
            
            await tcs.Task; // wait for welcome window to close
        }
        
        await LoadSave();
        StartupMainWindow();
    }

    private async Task LoadSave()
    {
        _desktop.MainWindow = new LoadingSaveWindow();
        _desktop.MainWindow.Show();

        saveService.LoadConfig();
        await saveService.LoadSave();
        
        _desktop.MainWindow.Close();
    }

    private void StartupMainWindow()
    {
        var mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
        };
        
        mainWindow.Closed += (_, _) => _desktop?.Shutdown(0);
        
        _desktop.MainWindow = mainWindow;
        _desktop.MainWindow.Show();
    }
}