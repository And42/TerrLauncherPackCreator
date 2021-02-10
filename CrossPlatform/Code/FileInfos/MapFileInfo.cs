using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public record MapFileInfo(
        string ResultFileName
    ) : IPackFileInfo;
}