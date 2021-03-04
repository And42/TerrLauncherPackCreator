using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos
{
    public record TranslationFileInfo(
        string Language
    ) : IPackFileInfo;
}