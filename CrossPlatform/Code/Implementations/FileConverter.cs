using System;
using System.IO;
using System.Threading.Tasks;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.FileInfos;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Json.FileInfos;
using CrossPlatform.Code.Utils;
using AudioFileInfo = CrossPlatform.Code.FileInfos.AudioFileInfo;

namespace CrossPlatform.Code.Implementations
{
    public class FileConverter : IFileConverter
    {
        private readonly ISessionHelper _sessionHelper;

        public FileConverter(
            ISessionHelper sessionHelper
        )
        {
            _sessionHelper = sessionHelper;
        }
        
        public async Task<(string convertedFile, string? configFile)> ConvertToTarget(
            FileType fileType,
            string sourceFile,
            IPackFileInfo? fileInfo
        )
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("File not found", sourceFile);

            string targetFile = _sessionHelper.GenerateNonExistentFilePath();
            IOUtils.EnsureParentDirExists(targetFile);

            // config
            string? configFile = null;
            if (fileInfo != null) {
                configFile = _sessionHelper.GenerateNonExistentFilePath();
                await File.WriteAllTextAsync(
                    configFile,
                    fileType switch
                    {
                        FileType.Texture => SerializeFileInfo((TextureFileInfo) fileInfo),
                        FileType.Map => SerializeFileInfo((MapFileInfo) fileInfo),
                        FileType.Character => SerializeFileInfo((CharacterFileInfo) fileInfo),
                        FileType.Gui => SerializeFileInfo((GuiFileInfo) fileInfo),
                        FileType.Translation => SerializeFileInfo((TranslationFileInfo) fileInfo),
                        FileType.Font => SerializeFileInfo((FontFileInfo) fileInfo),
                        FileType.Audio => SerializeFileInfo((AudioFileInfo) fileInfo),
                        _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
                    }
                );
            }

            // file
            File.Copy(sourceFile, targetFile, overwrite: false);
    
            return (targetFile, configFile);
        }

        public async Task<(string sourceFile, IPackFileInfo? fileInfo)> ConvertToSource(
            int packStructureVersion,
            FileType fileType,
            string targetFile,
            string? configFile
        )
        {
            if (!File.Exists(targetFile))
                throw new FileNotFoundException("File not found", targetFile);

            // config
            IPackFileInfo? fileInfo = null;
            if (configFile != null && File.Exists(configFile))
            {
                string configText = await File.ReadAllTextAsync(configFile);
                fileInfo = fileType switch
                {
                    FileType.Texture => DeserializeTextureFileInfo(configText),
                    FileType.Map => DeserializeMapFileInfo(configText),
                    FileType.Character => DeserializeCharacterFileInfo(configText),
                    FileType.Gui => DeserializeGuiFileInfo(configText),
                    FileType.Translation => DeserializeTranslationFileInfo(configText),
                    FileType.Font => DeserializeFontFileInfo(configText),
                    FileType.Audio => DeserializeAudioFileInfo(configText),
                    FileType.LastEnumElement => throw new ArgumentException(
                        (1 / (7 / (int) FileType.LastEnumElement)).ToString()
                    ),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (fileType == FileType.Texture && packStructureVersion < 15)
                {
                    var textureConfig = (TextureFileInfo) fileInfo;
                    if (textureConfig.Type == TextureFileInfo.TextureType.General)
                        fileInfo = textureConfig with { Animated = false };
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
                case FileType.Font:
                case FileType.Audio:
                    string uniqueFile = _sessionHelper.GenerateNonExistentFilePath();
                    IOUtils.EnsureParentDirExists(uniqueFile);
                    File.Copy(targetFile, uniqueFile, overwrite: false);
                    sourceFile = uniqueFile;
                    break;
                case FileType.LastEnumElement:
                    throw new ArgumentException((1 / (7 / (int) FileType.LastEnumElement)).ToString());
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }

            return (sourceFile, fileInfo);
        }

        private static TextureFileInfo DeserializeTextureFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<TextureFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new TextureFileInfo(
                Type: json.Type,
                EntryName: json.EntryName ?? string.Empty,
                ElementId: json.ElementId ?? 0,
                Animated: json.Animated ?? false,
                AnimateInGui: json.AnimateInGui ?? true,
                NumberOfVerticalFrames: json.NumberOfVerticalFrames ?? 1,
                NumberOfHorizontalFrames: json.NumberOfHorizontalFrames ?? 1,
                MillisecondsPerFrame: json.MillisecondsPerFrame ?? 100,
                ApplyOriginalSize: json.ApplyOriginalSize ?? true
            );
        }

        private static MapFileInfo DeserializeMapFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<MapFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new MapFileInfo(
                ResultFileName: json.ResultFileName
            );
        }
        
        private static CharacterFileInfo DeserializeCharacterFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<CharacterFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new CharacterFileInfo(
                ResultFileName: json.ResultFileName
            );
        }

        private static TextureFileInfo DeserializeGuiFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<GuiFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new GuiFileInfo(
                Type: json.Type,
                EntryName: json.EntryName ?? string.Empty,
                ElementId: json.ElementId ?? 0,
                Animated: json.Animated ?? false,
                AnimateInGui: json.AnimateInGui ?? true,
                NumberOfVerticalFrames: json.NumberOfVerticalFrames ?? 1,
                NumberOfHorizontalFrames: json.NumberOfHorizontalFrames ?? 1,
                MillisecondsPerFrame: json.MillisecondsPerFrame ?? 100,
                ApplyOriginalSize: json.ApplyOriginalSize ?? true
            );
        }
        
        private static TranslationFileInfo DeserializeTranslationFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<TranslationFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new TranslationFileInfo(
                Language: json.Language
            );
        }
        
        private static FontFileInfo DeserializeFontFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<FontFileInfoJson>(text) ?? throw new InvalidOperationException();;
            return new FontFileInfo(
                EntryName: json.EntryName ?? string.Empty
            );
        }
        
        private static AudioFileInfo DeserializeAudioFileInfo(string text)
        {
            var json = JsonUtils.Deserialize<AudioFileInfoJson>(text) ?? throw new InvalidOperationException();
            return new AudioFileInfo(
                EntryName: json.EntryName ?? throw new InvalidOperationException()
            );
        }

        public static string SerializeFileInfo(TextureFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new TextureFileInfoJson
                {
                    Type = fileInfo.Type,
                    EntryName = fileInfo.EntryName,
                    ElementId = fileInfo.ElementId,
                    Animated = fileInfo.Animated,
                    AnimateInGui = fileInfo.AnimateInGui,
                    NumberOfVerticalFrames = fileInfo.NumberOfVerticalFrames,
                    NumberOfHorizontalFrames = fileInfo.NumberOfHorizontalFrames,
                    MillisecondsPerFrame = fileInfo.MillisecondsPerFrame,
                    ApplyOriginalSize = fileInfo.ApplyOriginalSize
                }
            );
        }
        
        public static string SerializeFileInfo(MapFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new MapFileInfoJson
                {
                    ResultFileName = fileInfo.ResultFileName
                }
            );
        }
        
        public static string SerializeFileInfo(CharacterFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new CharacterFileInfoJson
                {
                    ResultFileName = fileInfo.ResultFileName
                }
            );
        }
        
        public static string SerializeFileInfo(GuiFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new GuiFileInfoJson
                {
                    Type = fileInfo.Type,
                    EntryName = fileInfo.EntryName,
                    ElementId = fileInfo.ElementId,
                    Animated = fileInfo.Animated,
                    AnimateInGui = fileInfo.AnimateInGui,
                    NumberOfVerticalFrames = fileInfo.NumberOfVerticalFrames,
                    NumberOfHorizontalFrames = fileInfo.NumberOfHorizontalFrames,
                    MillisecondsPerFrame = fileInfo.MillisecondsPerFrame,
                    ApplyOriginalSize = fileInfo.ApplyOriginalSize
                }
            );
        }
        
        public static string SerializeFileInfo(TranslationFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new TranslationFileInfoJson
                {
                    Language = fileInfo.Language
                }
            );
        }
        
        public static string SerializeFileInfo(FontFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new FontFileInfoJson
                {
                    EntryName = fileInfo.EntryName
                }
            );
        }
        
        public static string SerializeFileInfo(AudioFileInfo fileInfo)
        {
            return JsonUtils.Serialize(
                new AudioFileInfoJson
                {
                    EntryName = fileInfo.EntryName
                }
            );
        }
    }
}