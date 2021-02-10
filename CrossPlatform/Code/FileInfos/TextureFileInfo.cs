using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.FileInfos
{
    public record TextureFileInfo(
        TextureFileInfo.TextureType Type,
        string EntryName,
        int ElementId,
        bool Animated,
        bool AnimateInGui,
        int NumberOfVerticalFrames,
        int NumberOfHorizontalFrames,
        int MillisecondsPerFrame,
        bool ApplyOriginalSize
    ) : IPackFileInfo
    {
        public enum TextureType
        {
            General = 0,
            ItemDeprecated = 1,
            NpcDeprecated = 2,
            BuffDeprecated = 3,
            ExtraDeprecated = 4,
            Item = 5
        }
    }
}