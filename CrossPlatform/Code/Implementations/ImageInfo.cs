namespace CrossPlatform.Code.Implementations {

    public record ImageInfo(
        byte[] Bytes,
        ImageInfo.ImageType Type
    ) {
        public enum ImageType {
            Png = 0,
            Gif = 1
        }
    }
}