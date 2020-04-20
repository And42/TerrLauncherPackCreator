using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class TextureFileInfo : IPackFileInfo
    {
        [JsonProperty("entry_name")]
        public string EntryName { get; set; }
    }
}