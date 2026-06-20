using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using JADY.UI.Base;

namespace JADY.UI.Views.Dialogs;

public partial class LoginWindow : Window
{
    public bool EnteredPassword;
    public string? RawPassword;
    
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Password_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            EnteredPassword = true;
            RawPassword = Password.Text;
            Close();
        }
    }
}