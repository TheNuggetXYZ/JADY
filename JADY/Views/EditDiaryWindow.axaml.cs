using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using JADY.Models;
using JADY.ViewModels;

namespace JADY.Views;

public partial class EditDiaryWindow : Window
{
    public EditDiaryWindow()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
        {
            Submit_OnClick(null, null);
        }
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(new Diary()
        {
            Name = Name.Text,
        });
    }
}