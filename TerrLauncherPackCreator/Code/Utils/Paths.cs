using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class Paths
    {
#if DEBUG
        [NotNull]
        public static readonly string ExeDir = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
            "..", "Release"
        ));
#else
        [NotNull]
        public static readonly string ExeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        [NotNull]
        public static readonly string DataDir = Path.Combine(ExeDir, "data");

        [NotNull]
        public static readonly string TempDir = Path.Combine(ExeDir, "temp");

        [NotNull]
        public static readonly string TextureDefinitionsFile = Path.Combine(DataDir, "texture_definitions.txt");

        [NotNull]
        private static readonly string Etc2ConversionDir = Path.Combine(DataDir, "etc2_conversion");

        [NotNull]
        public static readonly string ConvertExe = Path.Combine(Etc2ConversionDir, "convert.exe");
        
        [NotNull]
        public static readonly string EtcPackExe = Path.Combine(Etc2ConversionDir, "etcpack.exe");
    }
}