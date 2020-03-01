using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class FileConverter : IFileConverter
    {
        private static readonly Dictionary<FileType, string> TempFilesDir = new Dictionary<FileType, string>
        {
            {FileType.Texture, "textures"},
            {FileType.Map, "maps"},
            {FileType.Character, "characters"},
            {FileType.Gui, "gui"}
        };

        [NotNull]
        public async Task<string> ConvertToTarget(FileType fileType, [NotNull] string sourceFile, [NotNull] string packTempDir)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("File not found", sourceFile);
            if (!Directory.Exists(packTempDir))
                Directory.CreateDirectory(packTempDir);

            var (_, initialFilesExt, convertedFilesExt, _) = PackUtils.PacksInfo.First(it => it.fileType == fileType);
            if (Path.GetExtension(sourceFile) != initialFilesExt)
                throw new ArgumentException($"`sourceFile` (`{sourceFile}`) has invalid extension");

            string tempFilesDir = Path.Combine(packTempDir, TempFilesDir[fileType]);
            Directory.CreateDirectory(tempFilesDir);

            switch (fileType)
            {
                case FileType.Texture:
                case FileType.Map:
                case FileType.Character:
                case FileType.Gui:
                    string targetFile = Path.Combine(tempFilesDir, Path.GetFileNameWithoutExtension(sourceFile) + convertedFilesExt);
                    File.Copy(sourceFile, targetFile, overwrite: true);
                    return targetFile;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }
    }
}