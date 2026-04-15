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
        if (value is Utils.DiaryEntryStatus status)
        {
            if (Application.Current is not null)
            {
                return status switch
                {
                    Utils.DiaryEntryStatus.None => Application.Current.Resources["RoundCheckIcon"],
                    Utils.DiaryEntryStatus.InProgress => Application.Current.Resources["CalendarEmptyIcon"],
                    Utils.DiaryEntryStatus.Completed => Application.Current.Resources["CalendarCheckmarkIcon"],
                    Utils.DiaryEntryStatus.Dropped => Application.Current.Resources["CalendarXMarkIcon"],
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