using Avalonia.Controls;
using Avalonia.Input;
using JADY.ViewModels;

namespace JADY.Views;

public partial class DiaryEntryControl : UserControl
{
    public DiaryEntryControl()
    {
        InitializeComponent();
    }

    private void DiaryEntryHeader_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Properties.IsLeftButtonPressed && DataContext is DiaryEntryViewModel vm)
            vm.IsExpanded = !vm.IsExpanded;
    }
}