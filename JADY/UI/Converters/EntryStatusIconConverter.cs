using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using JADY.Core.Data;

namespace JADY.UI.Converters;

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
                    EntryStatus.EventInProgress => Application.Current.Resources["CalendarEmptyIcon"],
                    EntryStatus.EventCompleted => Application.Current.Resources["CalendarCheckmarkIcon"],
                    EntryStatus.EventDropped => Application.Current.Resources["CalendarXMarkIcon"],
                    EntryStatus.LinkNote => Application.Current.Resources["LinkIcon"],
                    EntryStatus.LinkEndNote => Application.Current.Resources["LinkSlashIcon"],
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