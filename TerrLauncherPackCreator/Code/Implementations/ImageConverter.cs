using CrossPlatform.Code.Interfaces;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class ImageConverter : IImageConverter
    {
        public string ConvertWebPToTempPngFile(string webPPath)
        {
            return ImageUtils.ConvertWebPToTempPngFile(webPPath);
        }

        public string ConvertImageToTempWebPFile(string imagePath, bool lossless)
        {
            return ImageUtils.ConvertImageToTempWebPFile(imagePath, lossless);
        }

        public string ConvertImageToTempWebPFile(byte[] imageBytes, bool lossless)
        {
            return ImageUtils.ConvertImageToTempWebPFile(imageBytes, lossless);
        }
    }
}