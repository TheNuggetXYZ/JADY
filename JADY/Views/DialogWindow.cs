using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace JADY.Views;

public abstract class DialogWindow<T> : Window
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

    // Fix weird focus behavior when GetFirstFocusableElementOverride is not overriden
    protected override InputElement? GetFirstFocusableElementOverride() => FocusedElement();

    protected abstract InputElement? FocusedElement();

    protected override async void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
            await TrySubmitAsync();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        Dispatcher.UIThread.Post(() =>
        {
            Focus();
            
            FocusedElement()?.Focus();
            
        }, DispatcherPriority.Input);
    }
}