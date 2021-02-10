using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Implementations {

    public class ImageInfo {
        public enum ImageType {
            Png = 0,
            Gif = 1
        }
        
        [NotNull]
        public byte[] Bytes { get; }
        public ImageType Type { get; }
        
        public ImageInfo([NotNull] byte[] bytes, ImageType type) {
            Bytes = bytes;
            Type = type;
        }
    }
}