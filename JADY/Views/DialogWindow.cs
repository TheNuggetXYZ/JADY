using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace JADY.Views;

public abstract class DialogWindow<T> : Window
{
    protected DialogWindow()
    {
        AddHandler(KeyDownEvent, PreviewKeyDown, RoutingStrategies.Tunnel);
    }
    
    protected virtual async Task TrySubmitAsync() => await TrySubmitAsync(Optional<T>.Empty);

    protected virtual async Task TrySubmitAsync(Optional<T> value)
    {
        if (await CanSubmitAsync())
            await SubmitAsync(value);
    }

    protected virtual Task<bool> CanSubmitAsync() => Task.FromResult(true);

    /// <summary>
    /// Make sure to call TrySubmit for CanSubmit check to apply.
    /// </summary>
    protected virtual Task SubmitAsync() => SubmitAsync(Optional<T>.Empty);

    /// <summary>
    /// Make sure to call TrySubmit for CanSubmit check to apply.
    /// </summary>
    protected virtual Task SubmitAsync(Optional<T> value)
    {
        if (value.HasValue)
        {
            Close(value.Value);
        }
        else
        {
            var fallbackValue = GetValue();
            
            if (fallbackValue.HasValue)
                Close(fallbackValue);
            else
                Close(Optional<T>.Empty);
        }
        
        return Task.CompletedTask;
    }

    protected virtual Optional<T> GetValue() => Optional<T>.Empty;

    // Fix weird focus behavior when GetFirstFocusableElementOverride is not overriden
    protected override InputElement? GetFirstFocusableElementOverride() => FocusedElement();

    protected abstract InputElement? FocusedElement();

    private async void PreviewKeyDown(object? sender, KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            Close();
        }
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
        {
            e.Handled = true;
            await TrySubmitAsync();
        }
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

    // Ensure we dont return null
    protected new void Close()
    {
        base.Close(Optional<T>.Empty);
    }
    
    // Ensure we return the right type
    protected new void Close(T value)
    {
        base.Close(new Optional<T>(value));
    }
}