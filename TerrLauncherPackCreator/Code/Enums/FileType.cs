namespace TerrLauncherPackCreator.Code.Enums
{
    public enum FileType
    {
        Texture,
        Map,
        Character,
        Gui,
        Translation
        // todo: add
//        Audio,
//        Font,
    }
}

namespace TerrLauncherPackCreator.Code.Utils
{
    public static partial class PackUtils
    {
        public const int TotalFileTypes = 5;
    }
}