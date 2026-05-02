using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace JADY.Views;

public partial class YesNoDialogWindow : DialogWindow<bool>
{
    public YesNoDialogWindow()
    {
        InitializeComponent();
    }

    private void YesButton_OnClick(object? sender, RoutedEventArgs e) => Close(true);

    private void NoButton_OnClick(object? sender, RoutedEventArgs e) => Close(false);

    protected override InputElement? FocusedElement() => NoButton;
}