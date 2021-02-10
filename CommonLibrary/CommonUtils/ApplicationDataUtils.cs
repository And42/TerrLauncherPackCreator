using System;
using System.IO;

namespace CommonLibrary.CommonUtils
{
    public static class ApplicationDataUtils
    {
        public static string PathToRootFolder { get; }

        public static string PathToDataFolder { get; }

        public static string PathToSessionTempFolder { get; }

        static ApplicationDataUtils()
        {
            PathToRootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                // ReSharper disable once UnreachableCode
                "TerrLauncherPackCreator" + (CommonConstants.IsPreview ? "_preview" : "")
            );

            PathToDataFolder = Path.Combine(PathToRootFolder, "Data");
            PathToSessionTempFolder = Path.Combine(PathToRootFolder, "SessionTemp");
        }

        public static string GenerateNonExistentFilePath(string? extension = null)
        {
            string filePath;
            do
            {
                filePath = Path.Combine(PathToSessionTempFolder, $"{Guid.NewGuid().ToString()}{extension}");
            } while (File.Exists(filePath));

            return filePath;
        }

        public static string GenerateNonExistentDirPath()
        {
            string dirPath;
            do
            {
                dirPath = Path.Combine(PathToSessionTempFolder, Guid.NewGuid().ToString());
            } while (Directory.Exists(dirPath));

            return dirPath;
        }
    }
}
