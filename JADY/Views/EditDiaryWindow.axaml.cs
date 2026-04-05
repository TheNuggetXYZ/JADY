using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EditDiaryWindow : Window
{
    public EditDiaryWindow()
    {
        InitializeComponent();
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Diary()
        {
            Name = Name.Text,
        });
    }
}