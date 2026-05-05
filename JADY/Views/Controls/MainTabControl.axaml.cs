using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace JADY.Views.Controls;

public class MainTabControl : TabControl
{
    public static readonly StyledProperty<object?> LeftPanelContentProperty =
        AvaloniaProperty.Register<MainTabControl, object?>(nameof(LeftPanelContent));

    public object? LeftPanelContent
    {
        get => GetValue(LeftPanelContentProperty);
        set => SetValue(LeftPanelContentProperty, value);
    }

    public static readonly StyledProperty<object?> RightPanelContentProperty =
        AvaloniaProperty.Register<MainTabControl, object?>(nameof(RightPanelContent));

    public object? RightPanelContent
    {
        get => GetValue(RightPanelContentProperty);
        set => SetValue(RightPanelContentProperty, value);
    }
    
    public static readonly StyledProperty<object?> EmptyCollectionHintProperty =
        AvaloniaProperty.Register<MainTabControl, object?>(nameof(EmptyCollectionHint));

    public object? EmptyCollectionHint
    {
        get => GetValue(EmptyCollectionHintProperty);
        set => SetValue(EmptyCollectionHintProperty, value);
    }
}