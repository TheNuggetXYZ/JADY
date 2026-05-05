using Avalonia.Input;
using Avalonia.Interactivity;
using JADY.UI.Base;

namespace JADY.UI.Views.Dialogs;

public partial class YesNoDialogWindow : DialogWindow<bool>
{
    public YesNoDialogWindow(string message, string title)
    {
        InitializeComponent();
        
        Title = title;
        MessageText.Text = message;
    }

    private void YesButton_OnClick(object? sender, RoutedEventArgs e) => Close(true);

    private void NoButton_OnClick(object? sender, RoutedEventArgs e) => Close(false);

    protected override InputElement? FocusedElement() => NoButton;
}