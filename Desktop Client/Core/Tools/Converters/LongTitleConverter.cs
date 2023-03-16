using System;
using System.Globalization;
using System.Windows.Data;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class LongTitleConverter : IValueConverter
{
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
        int maxLenght = (int)parameter;

        var title = value.ToString();

        return title.Length > maxLenght 
                    ? title[0..maxLenght] + ".."
                    : title;
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}