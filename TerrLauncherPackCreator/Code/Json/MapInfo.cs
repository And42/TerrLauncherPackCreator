using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class MapInfo
    {
        [JsonProperty("result_file_name")]
        public string ResultFileName { get; set; }
    }
}