using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos
{

    public record AudioFileInfo(
        string EntryName
    ) : IPackFileInfo;
}