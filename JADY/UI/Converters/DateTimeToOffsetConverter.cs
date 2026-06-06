using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace JADY.UI.Converters;

public class DateTimeToOffsetConverter : IValueConverter
{
    // DateTimeOffset? -> DateTime?
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dto)
        {
            return dto.DateTime;
        }
        return null;
    }

    // DateTime? -> DateTimeOffset?
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            // Converts the local/unspecified calendar time to a clean offset
            return dt.Kind == DateTimeKind.Unspecified 
                ? new DateTimeOffset(dt, TimeSpan.Zero) 
                : new DateTimeOffset(dt);
        }
        return null;
    }
}