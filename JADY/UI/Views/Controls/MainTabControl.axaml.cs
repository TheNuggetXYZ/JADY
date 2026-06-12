using Avalonia;
using Avalonia.Controls;

namespace JADY.UI.Views.Controls;

public class MainTabControl : TabControl
{
    public static readonly StyledProperty<object?> AppHeaderProperty =
        AvaloniaProperty.Register<MainTabControl, object?>(nameof(AppHeader));

    public object? AppHeader
    {
        get => GetValue(AppHeaderProperty);
        set => SetValue(AppHeaderProperty, value);
    }
    
    public static readonly StyledProperty<object?> EmptyCollectionHintProperty =
        AvaloniaProperty.Register<MainTabControl, object?>(nameof(EmptyCollectionHint));

    public object? EmptyCollectionHint
    {
        get => GetValue(EmptyCollectionHintProperty);
        set => SetValue(EmptyCollectionHintProperty, value);
    }
}