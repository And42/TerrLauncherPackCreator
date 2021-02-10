using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace CrossPlatform.Code.Json.FileInfos
{
    internal class TranslationFileInfoJson : IPackFileInfo
    {
        [JsonProperty("language", Required = Required.Always)]
        public string Language = null!;
    }
}