using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class MapFileInfo : IPackFileInfo
    {
        [JsonProperty("result_file_name")]
        public string ResultFileName { get; set; }
    }
}