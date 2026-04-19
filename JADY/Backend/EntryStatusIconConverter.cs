using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace JADY.Backend;

public class EntryStatusIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Utils.EntryStatus status)
        {
            if (Application.Current is not null)
            {
                return status switch
                {
                    Utils.EntryStatus.OneTime => Application.Current.Resources["RoundCheckIcon"],
                    Utils.EntryStatus.InProgress => Application.Current.Resources["CalendarEmptyIcon"],
                    Utils.EntryStatus.Completed => Application.Current.Resources["CalendarCheckmarkIcon"],
                    Utils.EntryStatus.Dropped => Application.Current.Resources["CalendarXMarkIcon"],
                    _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                };
            }
            
        }

        return new BindingNotification("Unsupported value type");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}