using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;
using JADY.ViewModels;

namespace JADY.Views.Converters;

public class EntriesPanelTemplateConverter : IMultiValueConverter
{
    public DataTemplate? ListView { get; set; }
    public DataTemplate? Hint { get; set; }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (ListView is null || Hint is null)
            throw new NullReferenceException();

        if (values[0] is DiaryViewModel { Entries.Count: > 0 })
        {
            Console.WriteLine("list view");
            return ListView;
        }

        Console.WriteLine("hint view");
        return Hint;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}