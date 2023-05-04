using System;
using System.IO;
using System.Threading.Tasks;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.FileInfos;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Json.FileInfos;
using CrossPlatform.Code.Utils;
using AudioFileInfo = CrossPlatform.Code.FileInfos.AudioFileInfo;

namespace CrossPlatform.Code.Implementations;

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
            (1 / (8 / FileTypeEnum.Length)).Ignore();
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
                    FileType.Mod => SerializeFileInfo((ModFileInfo) fileInfo),
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
        if (!File.Exists(targetFile) && !Directory.Exists(targetFile))
            throw new FileNotFoundException("File not found", targetFile);

        // config
        IPackFileInfo? fileInfo = null;
        if (configFile != null && File.Exists(configFile))
        {
            string configText = await File.ReadAllTextAsync(configFile);
            (1 / (8 / FileTypeEnum.Length)).Ignore();
            fileInfo = fileType switch
            {
                FileType.Texture => DeserializeTextureFileInfo(configText),
                FileType.Map => DeserializeMapFileInfo(configText),
                FileType.Character => DeserializeCharacterFileInfo(configText),
                FileType.Gui => DeserializeGuiFileInfo(configText),
                FileType.Translation => DeserializeTranslationFileInfo(configText),
                FileType.Font => DeserializeFontFileInfo(configText),
                FileType.Audio => DeserializeAudioFileInfo(configText),
                FileType.Mod => DeserializeModFileInfo(configText),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
            if (fileType == FileType.Texture && packStructureVersion < 15)
            {
                var textureConfig = (TextureFileInfo) fileInfo;
                if (textureConfig.Type == TextureFileInfo.TextureType.General)
                    fileInfo = textureConfig with { Animated = false };
            }
        }

        (1 / (8 / FileTypeEnum.Length)).Ignore();
        string sourceFile = fileType switch
        {
            FileType.Texture or
                FileType.Map or
                FileType.Character or
                FileType.Gui or
                FileType.Translation or
                FileType.Font or
                FileType.Audio => ConvertFileToSource(targetFile),
            FileType.Mod => ConvertDirToSource(targetFile),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
        };

        return (sourceFile, fileInfo);
    }

    private static TextureFileInfo DeserializeTextureFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<TextureFileInfoJson>(text);
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
        var json = JsonUtils.Deserialize<MapFileInfoJson>(text);
        return new MapFileInfo(
            ResultFileName: json.ResultFileName
        );
    }
        
    private static CharacterFileInfo DeserializeCharacterFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<CharacterFileInfoJson>(text);
        return new CharacterFileInfo(
            ResultFileName: json.ResultFileName
        );
    }

    private static TextureFileInfo DeserializeGuiFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<GuiFileInfoJson>(text);
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
        var json = JsonUtils.Deserialize<TranslationFileInfoJson>(text);
        return new TranslationFileInfo(
            Language: json.Language,
            IgnoreForCategory: json.IgnoreForCategory
        );
    }
        
    private static FontFileInfo DeserializeFontFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<FontFileInfoJson>(text);
        return new FontFileInfo(
            EntryName: json.EntryName ?? string.Empty
        );
    }
        
    private static AudioFileInfo DeserializeAudioFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<AudioFileInfoJson>(text);
        return new AudioFileInfo(
            EntryName: json.EntryName ?? throw new InvalidOperationException()
        );
    }

    private static ModFileInfo DeserializeModFileInfo(string text)
    {
        var json = JsonUtils.Deserialize<ModFileInfoJson>(text);
        return new ModFileInfo(
            Id: json.Id,
            IgnoreForCategory: json.IgnoreForCategory
        );
    }

    private static string SerializeFileInfo(TextureFileInfo fileInfo)
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

    private static string SerializeFileInfo(MapFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new MapFileInfoJson
            {
                ResultFileName = fileInfo.ResultFileName
            }
        );
    }

    private static string SerializeFileInfo(CharacterFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new CharacterFileInfoJson
            {
                ResultFileName = fileInfo.ResultFileName
            }
        );
    }

    private static string SerializeFileInfo(GuiFileInfo fileInfo)
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

    private static string SerializeFileInfo(TranslationFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new TranslationFileInfoJson
            {
                Language = fileInfo.Language,
                IgnoreForCategory = fileInfo.IgnoreForCategory
            }
        );
    }

    private static string SerializeFileInfo(FontFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new FontFileInfoJson
            {
                EntryName = fileInfo.EntryName
            }
        );
    }

    private static string SerializeFileInfo(AudioFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new AudioFileInfoJson
            {
                EntryName = fileInfo.EntryName
            }
        );
    }
        
    private static string SerializeFileInfo(ModFileInfo fileInfo)
    {
        return JsonUtils.Serialize(
            new ModFileInfoJson
            {
                Id = fileInfo.Id,
                IgnoreForCategory = fileInfo.IgnoreForCategory
            }
        );
    }

    private string ConvertFileToSource(string file)
    {
        string uniqueFile = _sessionHelper.GenerateNonExistentFilePath();
        IOUtils.CopyFile(file, uniqueFile, overwrite: false);
        return uniqueFile;
    }
        
    private string ConvertDirToSource(string file)
    {
        string uniqueDir = _sessionHelper.GenerateNonExistentDirPath();
        IOUtils.CopyDirectory(file, uniqueDir, overwriteFiles: true);
        return uniqueDir;
    }
}