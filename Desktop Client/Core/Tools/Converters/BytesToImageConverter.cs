using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class BytesToImageConverter : IValueConverter
{
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
        var bytes = (byte[])value;

        BitmapImage image = new();

        if (bytes is null || bytes.Length == 0)
            return null;

        using (MemoryStream ms = new(bytes)) {
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
        }

        return image;
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}