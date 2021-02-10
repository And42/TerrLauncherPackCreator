namespace TerrLauncherPackCreator.Code.Interfaces
{
    public interface IZipHelper
    {
        void Extract(string inputZipPath, string targetDirectory);
    }
}