using System.Collections.Generic;
using System.Linq;
using CrossPlatform.Code.Enums;

namespace CrossPlatform.Code.Utils;

public static class PackUtils
{
    public record PackInfo(
        FileType FileType,
        string InitialFilesExt,
        string ConvertedFilesExt
    );
        
    public const int LatestPackStructureVersion = 25;
    public const int DefaultAuthorIconHeight = 70;
    public const string PacksExtension = ".tl";
    public const string PacksActualExtension = ".zip";
    public const string PackFileConfigExtension = ".json";

    public static IReadOnlyList<PackInfo> PacksInfo { get; }
    public static IReadOnlyList<string> TranslationLanguages { get; }
    public static IReadOnlyList<string> TranslationLanguageTitles { get; }
        
    static PackUtils()
    {
        (1 / (8 / FileTypeEnum.Length)).Ignore();

        PacksInfo = new PackInfo[]
        {
            new(FileType.Texture,     ".png",  ".texture"),
            new(FileType.Map,         ".wld",  ".world"),
            new(FileType.Character,   ".plr",  ".character"),
            new(FileType.Gui,         ".png",  ".gui"),
            new(FileType.Translation, ".json", ".translation"),
            new(FileType.Font,        ".png",  ".font"),
            new(FileType.Audio,       ".mp3",  ".audio"),
            new(FileType.Mod,         "",      ".mod")
        };
        TranslationLanguages = new[] { "en-US", "de-DE", "it-IT", "fr-FR", "es-ES", "ru-RU", "pt-BR", /*"zh-Hans", "pl-PL", "ja-JP"*/ };
        TranslationLanguageTitles = new[] { "English", "Deutsch", "Italiano", "Français", "Español", "Русский", "Português brasileiro" };
    }

    public static string GetInitialFilesExt(FileType fileType)
    {
        return PacksInfo.First(it => it.FileType == fileType).InitialFilesExt;
    }
        
    public static string GetConvertedFilesExt(FileType fileType)
    {
        return PacksInfo.First(it => it.FileType == fileType).ConvertedFilesExt;
    }
}