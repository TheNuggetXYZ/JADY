using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace JADY.Views.Windows;

public partial class MessageWindow : Window
{
    public MessageWindow(string message, string? title = null)
    {
        InitializeComponent();
        
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;
        
        MessageText.Text = message;
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e) => Close();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            Close();
        else if (e.Key == Key.Enter)
            Close();
    }
}