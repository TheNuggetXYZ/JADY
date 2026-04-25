using Avalonia.Controls;
using Avalonia.Input;

namespace JADY.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void TabStrip_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is ScrollViewer sv)
        {
            sv.Offset = sv.Offset.WithX(sv.Offset.X - e.Delta.Y * 50);
            e.Handled = true;
        }
    }
}