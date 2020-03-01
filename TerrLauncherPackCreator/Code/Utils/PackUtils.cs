using System.Collections.Generic;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class PackUtils
    {
        public const string PacksExtension = ".tl";
        
        public static IReadOnlyList<(FileType fileType, string initialFilesExt, string convertedFilesExt, string title)> PacksInfo { get; }

        static PackUtils()
        {
            PacksInfo = new[]
            {
                (FileType.Texture,    ".png", ".texture",   StringResources.PackTypeTextures),
                (FileType.Map,        ".wld", ".world",     StringResources.PackTypeMaps),
                (FileType.Character,  ".plr", ".character", StringResources.PackTypeCharacters),
                (FileType.Gui,        ".png", ".gui",       StringResources.PackTypeGui),
                // todo: add handling
//                (PackTypes.Audio,        ".tla",  ".ogg", ".audio",       StringResources.PackTypeAudio),
//                (PackTypes.Fonts,        ".tlf",  ".png", ".font",        StringResources.PackTypeFonts),
//                (PackTypes.Translations, ".tltr", ".txt", ".translation", StringResources.PackTypeTranslations),
            };
        }
    }
}
