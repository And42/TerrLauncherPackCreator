using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json {

    public class AudioFileInfo : IPackFileInfo
    {
        [JsonProperty("entry_name")]
        public string EntryName { get; set; }

        public AudioFileInfo() {}

        public AudioFileInfo(string entryName)
        {
            EntryName = entryName;
        }
    }
}