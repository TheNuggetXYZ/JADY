using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Models;

namespace JADY.Views;

public partial class AddDiaryWindow : Window
{
    public AddDiaryWindow()
    {
        InitializeComponent();
        
        Name.PropertyChanged += NameOnPropertyChanged;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();
        else if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.Control })
        {
            Submit();
        }
    }

    private void Submit_OnClick(object? sender, RoutedEventArgs e) => Submit();

    private void Submit()
    {
        if (!SubmitButton.IsEnabled)
            return;
        
        Close(new Diary()
        {
            Name = Name.Text,
        });
    }

    private void NameOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        SubmitButton.IsEnabled = !string.IsNullOrWhiteSpace(Name.Text);
    }
}