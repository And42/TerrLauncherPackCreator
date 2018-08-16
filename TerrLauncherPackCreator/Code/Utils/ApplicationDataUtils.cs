using System;
using System.IO;
using System.Reflection;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class ApplicationDataUtils
    {
        public static string PathToRootFolder { get; }

        public static string PathToDataFolder { get; }

        public static string PathToTempFolder { get; }

        static ApplicationDataUtils()
        {
            PathToRootFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Assembly.GetEntryAssembly().GetName().Name
            );

            PathToDataFolder = Path.Combine(PathToRootFolder, "Data");
            PathToTempFolder = Path.Combine(PathToRootFolder, "Temp");
        }
    }
}
