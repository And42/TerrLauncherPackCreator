using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class TextureInfo
    {
        [JsonProperty("entry_name")]
        public string EntryName { get; set; }
    }
}