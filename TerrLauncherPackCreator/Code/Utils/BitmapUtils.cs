using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace TerrLauncherPackCreator.Code.Utils;

public static class BitmapUtils
{
    public static Bitmap ToBitmap(this BitmapSource source)
    {
        var bmp = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
        BitmapData data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

        source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
        bmp.UnlockBits(data);

        return bmp;
    }

    public static BitmapSource ToBitmapSource(this Bitmap bitmap)
    {
        using (var stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Bmp);
            return stream.ToBitmapSource();
        }
    }
        
    public static BitmapSource ToBitmapSource(this Stream stream) {
        stream.Position = 0;
        BitmapImage result = new BitmapImage();
        result.BeginInit();
        // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
        // Force the bitmap to load right now so we can dispose the stream.
        result.CacheOption = BitmapCacheOption.OnLoad;
        result.StreamSource = stream;
        result.EndInit();
        result.Freeze();
        return result;
    }

    public static Bitmap ToBitmap(this byte[] imageBytes)
    {
        using (var memoryStream = new System.IO.MemoryStream(imageBytes))
        {
            return new Bitmap(memoryStream);
        }
    }

    public static byte[] ToByteArray(this Bitmap bitmap)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            bitmap.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }
    }
}