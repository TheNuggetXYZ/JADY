using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JADY.UI.Views.Windows;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Services;

public class AppStartupService(IServiceProvider serviceProvider, ISaveService saveService, IDiaryService diaryService, IShutdownService shutdownService) : IAppStartupService
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    
    public async Task AppStartup(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
        _desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown; // Make sure app doesnt shutdown
        
        if (!saveService.ExistsConfig()) 
            await ShowWelcomeWindow();
        
        await LoadSave();
        
        diaryService.LoadDiaries(false);
        
        StartupMainWindow();
    }

    private async Task ShowWelcomeWindow()
    {
        var welcomeWindow = serviceProvider.GetRequiredService<WelcomeWindow>();
        _desktop.MainWindow = welcomeWindow;
        
        var tcs = new TaskCompletionSource<bool>();
        welcomeWindow.Closed += (_, _) => tcs.SetResult(welcomeWindow.ContinueStartup);
        welcomeWindow.Show();
            
        bool continueStartup = await tcs.Task;
            
        if (!continueStartup)
            shutdownService.Shutdown();
    }

    private async Task LoadSave()
    {
        saveService.LoadConfig();
        await saveService.LoadSave();
    }

    private void StartupMainWindow()
    {
        var mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
        };
        
        mainWindow.Closed += (_, _) => shutdownService.Shutdown();
        
        _desktop.MainWindow = mainWindow;
        _desktop.MainWindow.Show();
    }
}