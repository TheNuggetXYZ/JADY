using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using JADY.Views.Base;
using JADY.Views.Dialogs;
using JADY.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Services;

public class WindowService(IServiceProvider serviceProvider) : IWindowService
{
    private readonly Dictionary<Type, Window> _openWindows = new();

    public Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        
        return null;
    }
    
    [Obsolete("This method is obsolete. Use OpenDialogWindowDI<T>() instead.")]
    public async Task<TResult?> OpenDialogWindow<T, TResult>(Window? owner, object? dataContext) where T : Window, new()
    {
        if (owner == null || _openWindows.TryGetValue(typeof(T), out var window))
            return default;
        
        T newWindow = new T {DataContext = dataContext};
        
        _openWindows.Add(typeof(T), newWindow);
        newWindow.Closed += (_, _) => { _openWindows.Remove(typeof(T)); };
        
        return await newWindow.ShowDialog<TResult>(owner);
    }

    public async Task<Optional<TResult>> OpenDialogWindowDI<T, TResult>(Window? owner) where T : Window
    {
        return await OpenDialogWindowDI<T, TResult, object>(owner, null);
    }
    
    public async Task<Optional<TResult>> OpenDialogWindowDI<T, TResult, TData>(Window? owner, TData? initData) where T : Window
    {
        if (owner == null || _openWindows.TryGetValue(typeof(T), out var window) || App.ServiceProvider is null)
            return default;
        
        T newWindow = serviceProvider.GetRequiredService<T>();

        if (initData is not null && newWindow is IDialogInitializable<TData> initializable)
        {
            initializable.Initialize(initData);
        }
        
        _openWindows.Add(typeof(T), newWindow);
        newWindow.Closed += (_, _) => { _openWindows.Remove(typeof(T)); };
        
        return await newWindow.ShowDialog<Optional<TResult>>(owner);
    }

    public void CloseWindow<T>() where T : Window
    {
        if (_openWindows.TryGetValue(typeof(T), out var window))
        {
            window.Close(); // gets automatically removed from dictionary
        }
    }

    public void OpenMessageBox(string message, string? title = null)
    {
        var box = new MessageWindow(message, title);
        
        box.Show();
    }

    public async Task<Optional<bool>> OpenYesNoMessageBox(Window? owner, string message, string title)
    {
        if (owner is null) return Optional<bool>.Empty;
        
        var box = new YesNoDialogWindow(message, title);
        
        return await box.ShowDialog<Optional<bool>>(owner);
    }
}