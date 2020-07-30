namespace CommonLibrary
{
    public static class CommonConstants
    {
        public const bool IsPreview = false;

        public const string VersionFileUrl = IsPreview
            // ReSharper disable once UnreachableCode
            ? "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/preview_version.txt"
            // ReSharper disable once UnreachableCode
            : "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/master_version.txt";
        
        public const string LatestVersionZipUrl = IsPreview
            // ReSharper disable once UnreachableCode
            ? "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/preview.zip"
            // ReSharper disable once UnreachableCode
            : "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/master.zip";
    }
}
