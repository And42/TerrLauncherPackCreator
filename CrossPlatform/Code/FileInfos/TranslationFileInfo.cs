using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public record TranslationFileInfo(
        string Language
    ) : IPackFileInfo;
}