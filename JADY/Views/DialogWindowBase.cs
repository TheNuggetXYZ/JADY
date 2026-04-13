using Avalonia.Controls;
using Avalonia.Input;

namespace JADY.Views;

public abstract class DialogWindowBase<T> : Window
{
    protected virtual void TrySubmit()
    {
        if (CanSubmit())
            Submit();
    }

    protected virtual bool CanSubmit() => true;

    /// <summary>
    /// Make sure to call TrySubmit for CanSubmit check to apply.
    /// </summary>
    protected virtual void Submit()
    {
        Close(GetValue());
    }

    protected abstract T GetValue();
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
            TrySubmit();
    }
}