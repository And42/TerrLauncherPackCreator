using System;
using System.IO;
using System.Threading.Tasks;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class FileConverter : IFileConverter
    {
        [NotNull]
        public async Task<(string convertedFile, string configFile)> ConvertToTarget(
            FileType fileType,
            [NotNull] string sourceFile,
            [CanBeNull] IPackFileInfo fileInfo
        )
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("File not found", sourceFile);

            string targetFile = ApplicationDataUtils.GenerateNonExistentFilePath();
            IOUtils.EnsureParentDirExists(targetFile);

            // config
            string configFile = null;
            if (fileInfo != null) {
                configFile = ApplicationDataUtils.GenerateNonExistentFilePath();
                File.WriteAllText(configFile, JsonConvert.SerializeObject(fileInfo, Formatting.Indented));
            }

            // file
            File.Copy(sourceFile, targetFile, overwrite: false);
    
            return (targetFile, configFile);
        }

        public async Task<(string sourceFile, IPackFileInfo fileInfo)> ConvertToSource(
            int packStructureVersion,
            FileType fileType,
            [NotNull] string targetFile,
            [CanBeNull] string configFile
        )
        {
            if (!File.Exists(targetFile))
                throw new FileNotFoundException("File not found", targetFile);

            {
                const int fileTypesHandled = 7;
                const int _ = 1 / (fileTypesHandled / PackUtils.TotalFileTypes) +
                              1 / (PackUtils.TotalFileTypes / fileTypesHandled);
            }

            // config
            IPackFileInfo fileInfo = null;
            if (configFile != null && File.Exists(configFile))
            {
                string configText = File.ReadAllText(configFile);
                fileInfo = fileType switch
                {
                    FileType.Texture => JsonConvert.DeserializeObject<TextureFileInfo>(configText),
                    FileType.Map => JsonConvert.DeserializeObject<MapFileInfo>(configText),
                    FileType.Character => JsonConvert.DeserializeObject<CharacterFileInfo>(configText),
                    FileType.Gui => JsonConvert.DeserializeObject<GuiFileInfo>(configText),
                    FileType.Translation => JsonConvert.DeserializeObject<TranslationFileInfo>(configText),
                    FileType.Font => JsonConvert.DeserializeObject<FontFileInfo>(configText),
                    FileType.Audio => JsonConvert.DeserializeObject<AudioFileInfo>(configText),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (fileType == FileType.Texture && packStructureVersion < 15)
                {
                    var textureConfig = (TextureFileInfo) fileInfo;
                    if (textureConfig.Type == TextureFileInfo.TextureType.General)
                        textureConfig.Animated = false;
                }
            }

            {
                const int fileTypesHandled = 7;
                const int _ = 1 / (fileTypesHandled / PackUtils.TotalFileTypes) +
                              1 / (PackUtils.TotalFileTypes / fileTypesHandled);
            }
            
            // file
            string sourceFile;
            switch (fileType)
            {
                case FileType.Texture:
                case FileType.Map:
                case FileType.Character:
                case FileType.Gui:
                case FileType.Translation:
                case FileType.Font:
                case FileType.Audio:
                    string uniqueFile = ApplicationDataUtils.GenerateNonExistentFilePath();
                    IOUtils.EnsureParentDirExists(uniqueFile);
                    File.Copy(targetFile, uniqueFile, overwrite: false);
                    sourceFile = uniqueFile;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }

            return (sourceFile, fileInfo);
        }
    }
}