using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using JADY.Core.Data;

namespace JADY.Services;

public class AppVisualService : IAppVisualService
{
    private static Style? _currentGlobalFontStyle;
    
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
    
    public void SetFont(string fontName)
    {
        if (Application.Current is App app)
        {
            var globalFont = FontManager.Current.SystemFonts.FirstOrDefault(x => x.Name == fontName);

            if (globalFont == null)
                return;
            
            if (_currentGlobalFontStyle != null)
            {
                app.Styles.Remove(_currentGlobalFontStyle);
            }
            
            _currentGlobalFontStyle = new Style(x => x.OfType<TextBlock>());
            
            _currentGlobalFontStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, globalFont));

            // 3. Inject the new style into the app
            app.Styles.Add(_currentGlobalFontStyle);
        }
    }
}