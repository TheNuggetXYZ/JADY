using Avalonia.Controls;
using Avalonia.Interactivity;

namespace JADY.Views;

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
}