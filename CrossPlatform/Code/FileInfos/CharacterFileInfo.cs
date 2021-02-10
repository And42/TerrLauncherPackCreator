using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public record CharacterFileInfo(
        string ResultFileName
    ) : IPackFileInfo;
}