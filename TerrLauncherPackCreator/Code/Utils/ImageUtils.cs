using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using WebPWrapper;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class ImageUtils
    {
        [NotNull]
        public static string ConvertWebPToTempPngFile([NotNull] string webPPath)
        {
            string tempIcon = ApplicationDataUtils.GenerateNonExistentFilePath(extension: ".png");
            IOUtils.EnsureParentDirExists(tempIcon);
            using (var webP = new WebP())
                webP.Load(webPPath).Save(tempIcon, ImageFormat.Png);
            return tempIcon;
        }
        
        [NotNull]
        public static string ConvertImageToTempWebPFile([NotNull] string imagePath, bool lossless)
        {
            using (var imageBitmap = new Bitmap(imagePath))
                return ConvertImageToTempWebPFile(imageBitmap, lossless);
        }
        
        [NotNull]
        public static string ConvertImageToTempWebPFile([NotNull] byte[] imageBytes, bool lossless)
        {
            using (var imageMemory = new MemoryStream(imageBytes))
            using (var imageBitmap = new Bitmap(imageMemory))
                return ConvertImageToTempWebPFile(imageBitmap, lossless);
        }
        
        [NotNull]
        public static string ConvertImageToTempWebPFile([NotNull] Bitmap imageBitmap, bool lossless)
        {
            string tempIcon = ApplicationDataUtils.GenerateNonExistentFilePath(extension: ".webp");
            IOUtils.EnsureParentDirExists(tempIcon);
            using (var webP = new WebP())
            {
                File.WriteAllBytes(
                    tempIcon,
                    lossless
                        ? webP.EncodeLossless(imageBitmap, speed: 9)
                        : webP.EncodeLossy(imageBitmap, quality: 75, speed: 9)
                );
            }

            return tempIcon;
        }
    }
}