using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class TranslationFileInfo : IPackFileInfo
    {
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}