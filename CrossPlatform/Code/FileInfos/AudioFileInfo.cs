using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{

    public record AudioFileInfo(
        string EntryName
    ) : IPackFileInfo;
}