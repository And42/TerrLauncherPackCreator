using System.IO;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class SafeFileSystemUtils
    {
        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        public static void DeleteDir(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
    }
}