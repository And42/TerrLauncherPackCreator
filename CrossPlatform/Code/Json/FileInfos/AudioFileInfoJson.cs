using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos
{

    internal class AudioFileInfoJson
    {
        [JsonProperty("entry_name", Required = Required.Always)]
        public string EntryName = null!;
    }
}