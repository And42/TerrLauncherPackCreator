using System;
using System.IO;

namespace TerrLauncherPackCreator.Code.Utils;

public static class Paths
{
#if DEBUG
    public static readonly string ExeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Release"));
#else
        public static readonly string ExeDir = AppContext.BaseDirectory;
#endif

    public static readonly string DataDir = Path.Combine(ExeDir, "data");
    public static readonly string TempDir = Path.Combine(ExeDir, "temp");
    public static readonly string AuthorsFile = Path.Combine(DataDir, "authors.json");
    public static readonly string AppSettingsFile = Path.Combine(DataDir, "appSettings.json");
}