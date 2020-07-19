namespace CommonLibrary
{
    public static class CommonConstants
    {
        public const bool IsPreview = true;

        public const string VersionFileUrl = IsPreview
            // ReSharper disable once UnreachableCode
            ? "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/prevew_version.txt"
            // ReSharper disable once UnreachableCode
            : "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/master_version.txt";
        
        public const string LatestVersionZipUrl = IsPreview
            // ReSharper disable once UnreachableCode
            ? "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/prevew.zip"
            // ReSharper disable once UnreachableCode
            : "https://terrlauncher.ams3.cdn.digitaloceanspaces.com/pc_pack_creator/master.zip";
    }
}
