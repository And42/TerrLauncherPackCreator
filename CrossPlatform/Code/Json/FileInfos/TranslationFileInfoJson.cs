using CrossPlatform.Code.Interfaces;
using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos
{
    internal class TranslationFileInfoJson : IPackFileInfo
    {
        [JsonProperty("language", Required = Required.Always)]
        public string Language = null!;
    }
}