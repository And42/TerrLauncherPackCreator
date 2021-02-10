using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json {

    public record FontFileInfo(
        string EntryName
    ) : IPackFileInfo;
}