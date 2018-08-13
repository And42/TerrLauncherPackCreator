using System.Collections.Generic;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class PackUtils
    {
        public static IReadOnlyList<(PackTypes packType, string packExt, string packFilesExt, string title)> PacksInfo { get; }

        static PackUtils()
        {
            PacksInfo = new[]
            {
                (PackTypes.Textures,    ".tlt",  ".png",    StringResources.PackTypeTextures),
                (PackTypes.Maps,        ".tlm",  ".world",  StringResources.PackTypeMaps),
                (PackTypes.Characters,  ".tlc",  ".player", StringResources.PackTypeCharacters),
                (PackTypes.Audio,       ".tla",  ".ogg",    StringResources.PackTypeAudio),
                (PackTypes.Fonts,       ".tlf",  ".png",    StringResources.PackTypeFonts),
                (PackTypes.Gui,         ".tlg",  ".png",    StringResources.PackTypeGui),
                (PackTypes.Translations, ".tltr", ".txt",   StringResources.PackTypeTranslations),
                // todo: add in the future
                //(PackTypes.Global, ".tlgl", "")
            };
        }
    }
}
