using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;

namespace JADY.Views;

public abstract class DialogWindowBase<T> : Window
{
    protected virtual async Task TrySubmitAsync()
    {
        if (await CanSubmitAsync())
            await SubmitAsync();
    }

    protected virtual Task<bool> CanSubmitAsync() => Task.FromResult(true);

    /// <summary>
    /// Make sure to call TrySubmit for CanSubmit check to apply.
    /// </summary>
    protected virtual Task SubmitAsync()
    {
        Close(GetValue());
        return Task.CompletedTask;
    }

    protected abstract T GetValue();
    
    protected override async void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
            await TrySubmitAsync();
    }
}