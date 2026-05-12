using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace JADY.UI.Views.Windows;

public partial class MessageWindow : Window
{
    // Required for the compiler and previewer
    public MessageWindow()
    {
        InitializeComponent();
    }
    
    public MessageWindow(string message, string? title = null) : this()
    {
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