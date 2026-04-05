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
    public static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        
        return null;
    }
    
    public static async Task OpenDialogWindow<T>(Window? owner, Action? onCloseAction, object? dataContext) where T : Window, new()
    {
        if (owner == null) return;
        
        T newWindow = new T {DataContext = dataContext};
        await newWindow.ShowDialog(owner);
        onCloseAction?.Invoke();
    }
}