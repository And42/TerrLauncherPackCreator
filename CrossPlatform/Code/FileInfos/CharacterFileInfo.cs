using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos
{
    public record CharacterFileInfo(
        string ResultFileName
    ) : IPackFileInfo;
}