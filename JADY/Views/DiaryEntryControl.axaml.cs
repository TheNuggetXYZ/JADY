using Avalonia.Controls;
using Avalonia.Interactivity;
using JADY.ViewModels;

namespace JADY.Views;

public partial class DiaryEntryControl : UserControl
{
    public DiaryEntryControl()
    {
        InitializeComponent();
    }

    private void Control_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DiaryEntryViewModel vm)
        {
            vm.IsExpanded = !vm.IsExpanded;

            ExpandAngle.Classes.Set("isExpanded", vm.IsExpanded);
        }
    }
}