using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace JADY.UI.Views.Controls;

public class IconToggleButton : ToggleButton
{
    public static readonly StyledProperty<StreamGeometry?> IconDataProperty =
        AvaloniaProperty.Register<IconButton, StreamGeometry?>(nameof(IconData));

    public StreamGeometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
    
    public static readonly StyledProperty<StreamGeometry?> CheckedIconDataProperty =
        AvaloniaProperty.Register<IconButton, StreamGeometry?>(nameof(CheckedIconData));

    public StreamGeometry? CheckedIconData
    {
        get => GetValue(CheckedIconDataProperty);
        set => SetValue(CheckedIconDataProperty, value);
    }
    
    public static readonly StyledProperty<object?> IconTextProperty =
        AvaloniaProperty.Register<IconButton, object?>(nameof(IconText));

    public object? IconText
    {
        get => GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }
    
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<IconButton, double>(nameof(IconSize), defaultValue: 18);

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}