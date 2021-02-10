namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IImageConverter
    {
        string ConvertWebPToTempPngFile(string webPPath);
        string ConvertImageToTempWebPFile(string imagePath, bool lossless);
        string ConvertImageToTempWebPFile(byte[] imageBytes, bool lossless);
    }
}