using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Desktop_Client.Core.Tools;

internal static class ImageConverter
{
    internal static BitmapImage CreateImageFromFile(string fileName)
    {
        BitmapImage picture = new();

        picture.BeginInit();

        picture.UriSource = new(fileName);
        picture.CacheOption = BitmapCacheOption.OnLoad;

        picture.EndInit();

        return picture;
    }

    internal static BitmapImage ImageFromBytes(byte[] bytes)
    {
        BitmapImage image = new();

        if (bytes is null || bytes.Length == 0)
            return null;

        using (MemoryStream ms = new(bytes))
        {
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
        }

        return image;
    }

    internal static byte[] BytesFromImage(BitmapImage image)
    {
        if (image is null)
            return Array.Empty<byte>();

        JpegBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(image));

        byte[] imageBytes;

        using (MemoryStream stream = new())
        {
            encoder.Save(stream);
            imageBytes = stream.ToArray();
        }

        return imageBytes;
    }
}