using Avalonia;
using Avalonia.Styling;
using JADY.Core.Data;

namespace JADY.Services;

public class AppVisualService : IAppVisualService
{
    public void SetTheme(AppTheme theme)
    {
        if (Application.Current is App app)
        {
            app.RequestedThemeVariant = theme switch
            {
                AppTheme.System => ThemeVariant.Default,
                AppTheme.Dark => ThemeVariant.Dark,
                AppTheme.Light => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        }
    }
}