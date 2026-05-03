using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.Core.Data;
using JADY.Views.Base;

namespace JADY.Views.Dialogs;

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

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e) => Close();
}