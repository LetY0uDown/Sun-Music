using System;
using System.Globalization;
using System.Windows.Data;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class BytesToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var bytes = (byte[])value;

        return ImageConverter.ImageFromBytes(bytes);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}