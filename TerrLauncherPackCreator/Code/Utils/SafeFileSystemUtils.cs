using System.IO;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class SafeFileSystemUtils
    {
        public static void DeleteFile([NotNull] string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        public static void DeleteDir([NotNull] string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}