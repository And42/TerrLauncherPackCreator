using System;
using System.IO;
using System.Threading.Tasks;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class FileConverter : IFileConverter
    {
        [NotNull]
        public async Task<(string convertedFile, string configFile)> ConvertToTarget(FileType fileType, [NotNull] string sourceFile, [CanBeNull] IPackFileInfo fileInfo)
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

        public async Task<(string sourceFile, IPackFileInfo fileInfo)> ConvertToSource(FileType fileType, [NotNull] string targetFile, [CanBeNull] string configFile)
        {
            if (!File.Exists(targetFile))
                throw new FileNotFoundException("File not found", targetFile);

            // config
            IPackFileInfo fileInfo = null;
            if (configFile != null && File.Exists(configFile)) {
                switch (fileType) {
                    case FileType.Texture:
                        fileInfo = JsonConvert.DeserializeObject<TextureFileInfo>(File.ReadAllText(configFile));
                        break;
                    case FileType.Map:
                        fileInfo = JsonConvert.DeserializeObject<MapFileInfo>(File.ReadAllText(configFile));
                        break;
                    case FileType.Character:
                        fileInfo = JsonConvert.DeserializeObject<CharacterFileInfo>(File.ReadAllText(configFile));
                        break;
                    case FileType.Gui:
                        fileInfo = JsonConvert.DeserializeObject<GuiFileInfo>(File.ReadAllText(configFile));
                        break;
                    case FileType.Translation:
                        fileInfo = JsonConvert.DeserializeObject<TranslationFileInfo>(File.ReadAllText(configFile));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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