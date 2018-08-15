using System;
using System.IO;
using System.Reflection;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class ApplicationDataUtils
    {
        public static string PathToDataFolder { get; }

        public static string PathToTempFolder { get; }

        static ApplicationDataUtils()
        {
            PathToDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Assembly.GetEntryAssembly().GetName().Name
            );

            PathToTempFolder = Path.Combine(PathToDataFolder, "Temp");
        }
    }
}
