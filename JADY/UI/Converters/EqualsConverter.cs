using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace JADY.UI.Converters;

public class EqualsConverter : IValueConverter
{
    public int CompareTo { get; set; }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
            return i == CompareTo;
        else if (value is float f)
            return f == CompareTo;

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}