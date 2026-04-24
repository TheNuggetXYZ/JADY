using Avalonia;

namespace JADY.Backend;

public class AppVisualService : IAppVisualService
{
    public void SetTheme(bool isDark)
    {
        if (Application.Current is App app)
        {
            app.RequestedThemeVariant = isDark ? Avalonia.Styling.ThemeVariant.Dark : Avalonia.Styling.ThemeVariant.Light;
        }
    }
}