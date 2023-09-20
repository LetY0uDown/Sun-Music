using Material.Icons;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Desktop_Client.Core.Tools.Converters;

internal sealed class IsFavoriteConverter : IValueConverter
{
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
        var trackID = (Guid)value;

        return App.FaviriteTracksIDs.Contains(trackID) ? MaterialIconKind.Heart
                                                       : MaterialIconKind.HeartOutline;
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}