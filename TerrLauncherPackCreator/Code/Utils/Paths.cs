using System;
using System.IO;

namespace TerrLauncherPackCreator.Code.Utils;

public static class Paths
{
    public static readonly string TempDir;
    public static readonly string AuthorsFile;
    public static readonly string AppSettingsFile;

    static Paths()
    {
#if DEBUG
        string exeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Release"));
#else
        string exeDir = AppContext.BaseDirectory;
#endif
        string dataDir = Path.Combine(exeDir, "data");

        TempDir = Path.Combine(exeDir, "temp");
        AuthorsFile = Path.Combine(dataDir, "authors.json");
        AppSettingsFile = Path.Combine(dataDir, "appSettings.json");
    }
}