using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace JADY.UI.Views.Controls;

public class AppHeader : TemplatedControl
{
    public static readonly StyledProperty<object?> DiaryPanelProperty =
        AvaloniaProperty.Register<AppHeader, object?>(nameof(DiaryPanel));

    public object? DiaryPanel
    {
        get => GetValue(DiaryPanelProperty);
        set => SetValue(DiaryPanelProperty, value);
    }
}