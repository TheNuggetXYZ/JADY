using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;
using JADY.ViewModels;

namespace JADY.Views.Converters;

public class EntriesPanelTemplateConverter : IValueConverter
{
    public DataTemplate? ListView { get; set; }
    public DataTemplate? Hint { get; set; }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (ListView is null || Hint is null)
            throw new NullReferenceException();
        
        if (value is DiaryViewModel { Entries.Count: > 0 })
        {
            return ListView;
        }
        
        return Hint;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}