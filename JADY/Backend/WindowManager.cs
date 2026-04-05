using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using JADY.Views;

namespace JADY.Backend;

public static class WindowManager
{
    
    private static readonly Dictionary<Type, Window> OpenWindows = new();

    public static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        
        return null;
    }
    
    public static async Task OpenDialogWindow<T>(Window? owner, Action? onCloseAction, object? dataContext) where T : Window, new()
    {
        if (owner == null) return;

        if (OpenWindows.TryGetValue(typeof(T), out var window))
            return;
        
        T newWindow = new T {DataContext = dataContext};
        
        OpenWindows.Add(typeof(T), newWindow);
        newWindow.Closed += (_, _) => { OpenWindows.Remove(typeof(T)); };
        
        await newWindow.ShowDialog(owner);
        onCloseAction?.Invoke();
    }

    public static void CloseWindow<T>() where T : Window
    {
        if (OpenWindows.TryGetValue(typeof(T), out var window))
        {
            window.Close(); // gets automatically removed from dictionary
        }
    }
}