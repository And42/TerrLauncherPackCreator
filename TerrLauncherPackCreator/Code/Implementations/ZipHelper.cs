using Ionic.Zip;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class ZipHelper : IZipHelper
    {
        public void Extract(string inputZipPath, string targetDirectory)
        {
            using (var zip = ZipFile.Read(inputZipPath))
                zip.ExtractAll(targetDirectory);
        }
    }
}