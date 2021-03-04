using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos
{
    public record MapFileInfo(
        string ResultFileName
    ) : IPackFileInfo;
}