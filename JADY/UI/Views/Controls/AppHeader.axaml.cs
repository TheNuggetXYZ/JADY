using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace JADY.UI.Views.Controls;

public class AppHeader : TemplatedControl
{
    public static readonly StyledProperty<object?> LeftPanelContentProperty =
        AvaloniaProperty.Register<AppHeader, object?>(nameof(LeftPanelContent));

    public object? LeftPanelContent
    {
        get => GetValue(LeftPanelContentProperty);
        set => SetValue(LeftPanelContentProperty, value);
    }

    public static readonly StyledProperty<object?> RightPanelContentProperty =
        AvaloniaProperty.Register<AppHeader, object?>(nameof(RightPanelContent));

    public object? RightPanelContent
    {
        get => GetValue(RightPanelContentProperty);
        set => SetValue(RightPanelContentProperty, value);
    }

    public static readonly StyledProperty<object?> ItemsPanelProperty =
        AvaloniaProperty.Register<AppHeader, object?>(nameof(ItemsPanel));

    public object? ItemsPanel
    {
        get => GetValue(RightPanelContentProperty);
        set => SetValue(RightPanelContentProperty, value);
    }
}