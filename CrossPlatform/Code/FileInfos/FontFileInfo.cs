using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos {

    public record FontFileInfo(
        string EntryName
    ) : IPackFileInfo;
}