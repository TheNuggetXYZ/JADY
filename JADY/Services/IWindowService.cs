using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;

namespace JADY.Services;

public interface IWindowService
{
    Window? GetMainWindow();
    
    Task<Optional<TResult>> OpenDialogWindowDI<T, TResult, TData>(Window? owner, TData? initData) where T : Window;

    Task<Optional<TResult>> OpenDialogWindowDI<T, TResult>(Window? owner) where T : Window;

    void OpenMessageBox(string message, string? title = null);
    
    Task<Optional<bool>> OpenYesNoMessageBox(Window? owner, string message, string title);

    void CloseWindow<T>() where T : Window;
}