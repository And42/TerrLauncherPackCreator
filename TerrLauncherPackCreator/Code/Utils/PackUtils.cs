using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class PackUtils
    {
        public const int DefaultAuthorIconHeight = 70;
        public const string PacksExtension = ".tl";
        public const string PacksActualExtension = ".zip";
        public const string PackFileConfigExtension = ".json";

        [NotNull]
        public static IReadOnlyList<(FileType fileType, string initialFilesExt, string convertedFilesExt, string title)> PacksInfo { get; }
        [NotNull]
        public static IReadOnlyList<string> TranslationLanguages { get; }
        [NotNull]
        public static IReadOnlyList<string> TranslationLanguageTitles { get; }
        
        static PackUtils()
        {
#pragma warning disable 219
            const int _ = 1 / (7 / (int) FileType.LastEnumElement);
#pragma warning restore 219
            
            PacksInfo = new[]
            {
                (FileType.Texture,     ".png",  ".texture",     StringResources.PackTypeTextures),
                (FileType.Map,         ".wld",  ".world",       StringResources.PackTypeMaps),
                (FileType.Character,   ".plr",  ".character",   StringResources.PackTypeCharacters),
                (FileType.Gui,         ".png",  ".gui",         StringResources.PackTypeGui),
                (FileType.Translation, ".json", ".translation", StringResources.PackTypeTranslations),
                (FileType.Font,        ".png",  ".font",        StringResources.PackTypeFonts),
                (FileType.Audio,       ".mp3",  ".audio",       StringResources.PackTypeAudio)
            };
            TranslationLanguages = new[] { "en-US", "de-DE", "it-IT", "fr-FR", "es-ES", "ru-RU", "pt-BR", /*"zh-Hans", "pl-PL", "ja-JP"*/ };
            TranslationLanguageTitles = new[] { "English", "Deutsch", "Italiano", "Français", "Español", "Русский", "Português brasileiro" };
        }

        [NotNull]
        public static string GetInitialFilesExt(FileType fileType)
        {
            return PacksInfo.First(it => it.fileType == fileType).initialFilesExt;
        }
        
        [NotNull]
        public static string GetConvertedFilesExt(FileType fileType)
        {
            return PacksInfo.First(it => it.fileType == fileType).convertedFilesExt;
        }
    }
}
