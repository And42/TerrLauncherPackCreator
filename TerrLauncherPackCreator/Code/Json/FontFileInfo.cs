using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json {

    public class FontFileInfo : IPackFileInfo
    {
        [JsonProperty("entry_name")]
        public string EntryName { get; set; }

        public FontFileInfo() {}

        public FontFileInfo(string entryName)
        {
            EntryName = entryName;
        }
    }
}