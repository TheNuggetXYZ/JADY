using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JADY.Models;

namespace JADY.Views;

public partial class AddDiaryWindow : Window
{
    public AddDiaryWindow()
    {
        InitializeComponent();
    }

    private void SubmitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Diary()
        {
            Name = Name.Text,
        });
    }
}