using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace TerrLauncherPackCreator.Code.Utils
{
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

        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                source.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()).FreezeIfCan();
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
}
