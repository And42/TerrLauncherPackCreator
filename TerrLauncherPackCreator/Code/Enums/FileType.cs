namespace TerrLauncherPackCreator.Code.Enums
{
    public enum FileType
    {
        Texture,
        Map,
        Character,
        Gui,
        Translation,
        Font
        // todo: add
//        Audio,
    }
}

namespace TerrLauncherPackCreator.Code.Utils
{
    public static partial class PackUtils
    {
        public const int TotalFileTypes = 6;
    }
}