using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos
{
    internal class MapFileInfoJson
    {
        [JsonProperty("result_file_name", Required = Required.Always)]
        public string ResultFileName = null!;
    }
}