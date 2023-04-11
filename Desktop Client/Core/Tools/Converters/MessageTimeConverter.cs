using System;
using System.Globalization;
using System.Windows.Data;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class MessageTimeConverter : IValueConverter
{
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
        var date = (DateTime)value;

        if ((DateTime.Now - date).Days > 1 ) {
            return date.ToString("dd/MM/yyyy HH:mm");
        } else {
            return date.ToString("HH:mm");
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}