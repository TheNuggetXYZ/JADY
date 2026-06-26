using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

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
    
    public static readonly StyledProperty<EventHandler<PointerPressedEventArgs>?> AppHeaderPointerPressedProperty =
        AvaloniaProperty.Register<MainTabControl, EventHandler<PointerPressedEventArgs>?>(nameof(AppHeaderPointerPressed));

    public EventHandler<PointerPressedEventArgs>? AppHeaderPointerPressed
    {
        get => GetValue(AppHeaderPointerPressedProperty);
        set => SetValue(AppHeaderPointerPressedProperty, value);
    }
    
    public static readonly StyledProperty<EventHandler<TappedEventArgs>?> AppHeaderDoubledTappedProperty =
        AvaloniaProperty.Register<MainTabControl, EventHandler<TappedEventArgs>?>(nameof(AppHeaderDoubledTapped));

    public EventHandler<TappedEventArgs>? AppHeaderDoubledTapped
    {
        get => GetValue(AppHeaderDoubledTappedProperty);
        set => SetValue(AppHeaderDoubledTappedProperty, value);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var dragRegion = e.NameScope.Find<Border>("PART_AppHeader");
        
        if (dragRegion != null)
        {
            dragRegion.PointerPressed += (sender, args) =>
            {
                AppHeaderPointerPressed?.Invoke(sender, args);
            };

            dragRegion.DoubleTapped += (sender, args) =>
            {
                AppHeaderDoubledTapped?.Invoke(sender, args);
            };
        }
    }
}