using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace JADY.UI.Views.Controls;

public class IconButton : HoverButton
{
    public static readonly StyledProperty<object?> IconDataProperty =
        AvaloniaProperty.Register<IconButton, object?>(nameof(IconData));

    public object? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
    
    public static readonly StyledProperty<object?> IconTextProperty =
        AvaloniaProperty.Register<IconButton, object?>(nameof(IconText));

    public object? IconText
    {
        get => GetValue(IconTextProperty);
        set => SetValue(IconTextProperty, value);
    }
}