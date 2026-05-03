using System;
using System.Globalization;
using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;

namespace JADY.Backend;

public class SaveIconTemplateConverter : IValueConverter
{
    public DataTemplate? Saved { get; set; }
    public DataTemplate? Unsaved { get; set; }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool unsavedChanges)
        {
            if (Saved is not null && Unsaved is not null)
                return unsavedChanges ? Unsaved.Build(null) : Saved.Build(null);
            
            throw new InvalidOperationException();
        }
        
        throw new InvalidCastException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}