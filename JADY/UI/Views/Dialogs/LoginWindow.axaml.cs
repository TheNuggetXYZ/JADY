using Avalonia.Data;
using Avalonia.Input;
using JADY.UI.Base;

namespace JADY.UI.Views.Dialogs;

public partial class LoginWindow : DialogWindow<string?>
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    protected override InputElement? FocusedElement() => Password;

    protected override Optional<string?> GetValue() => Password.Text;

    private async void Password_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            await TrySubmitAsync();
    }
}