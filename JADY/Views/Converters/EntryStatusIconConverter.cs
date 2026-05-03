using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using JADY.Core.Data;

namespace JADY.Views.Converters;

public class EntryStatusIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EntryStatus status)
        {
            if (Application.Current is not null)
            {
                return status switch
                {
                    EntryStatus.OneTime => Application.Current.Resources["RoundCheckIcon"],
                    EntryStatus.InProgress => Application.Current.Resources["CalendarEmptyIcon"],
                    EntryStatus.Completed => Application.Current.Resources["CalendarCheckmarkIcon"],
                    EntryStatus.Dropped => Application.Current.Resources["CalendarXMarkIcon"],
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