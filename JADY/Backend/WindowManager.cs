using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

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
    
    public static async Task<TResult> OpenDialogWindow<T, TResult>(Window? owner, object? dataContext) where T : Window, new()
    {
        if (owner == null || OpenWindows.TryGetValue(typeof(T), out var window))
            return default;
        
        T newWindow = new T {DataContext = dataContext};
        
        OpenWindows.Add(typeof(T), newWindow);
        newWindow.Closed += (_, _) => { OpenWindows.Remove(typeof(T)); };
        
        return await newWindow.ShowDialog<TResult>(owner);
    }

    public static void CloseWindow<T>() where T : Window
    {
        if (OpenWindows.TryGetValue(typeof(T), out var window))
        {
            window.Close(); // gets automatically removed from dictionary
        }
    }

    public static async Task OpenMessageBox(Window? owner, string title, string message)
    {
        if (owner == null)
            return;
        
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok);

        await box.ShowWindowDialogAsync(owner);
    }
}