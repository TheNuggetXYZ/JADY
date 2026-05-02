using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Data;

namespace JADY.Views;

public partial class UnsavedChangesWindow : DialogWindow<UnsavedChangesChoice>
{
    public UnsavedChangesWindow()
    {
        InitializeComponent();

        SaveButton.Focus();
    }

    protected override InputElement? FocusedElement() => SaveButton;

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync(UnsavedChangesChoice.Save);

    private async void QuitButton_OnClick(object? sender, RoutedEventArgs e) => await TrySubmitAsync(UnsavedChangesChoice.Quit);

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}