using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos;

public record ModFileInfo(
    string Id,
    bool IgnoreForCategory
) : IPackFileInfo;