using System.Diagnostics.CodeAnalysis;

namespace TerrLauncherPackCreator.Code.Json {

    public record GuiFileInfo : TextureFileInfo
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public GuiFileInfo(
            TextureType Type,
            string EntryName,
            int ElementId,
            bool Animated,
            bool AnimateInGui,
            int NumberOfVerticalFrames,
            int NumberOfHorizontalFrames,
            int MillisecondsPerFrame,
            bool ApplyOriginalSize
        ) : base(
            Type,
            EntryName,
            ElementId,
            Animated,
            AnimateInGui,
            NumberOfVerticalFrames,
            NumberOfHorizontalFrames,
            MillisecondsPerFrame,
            ApplyOriginalSize
        ) { }
    }
}