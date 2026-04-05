using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace JADY.Views;

public partial class DiaryEntryControl : UserControl
{
    public bool IsDiaryEntryOpen = false;
    
    public DiaryEntryControl()
    {
        InitializeComponent();
        SetVisibilityValues();
    }

    private void DiaryEntryHeader_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Properties.IsLeftButtonPressed)
        {
            IsDiaryEntryOpen = !IsDiaryEntryOpen;

            SetVisibilityValues();
        }
    }

    private void SetVisibilityValues()
    {
        ObjectToOpen0.IsVisible = ObjectToOpen0.IsEnabled = IsDiaryEntryOpen;
        ObjectToOpen1.IsVisible = ObjectToOpen1.IsEnabled = IsDiaryEntryOpen;
    }
}