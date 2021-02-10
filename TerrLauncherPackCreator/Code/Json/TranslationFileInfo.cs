using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Json
{
    public class TranslationFileInfo : IPackFileInfo
    {
        [JsonProperty("language", Required = Required.Always)]
        public string Language { get; set; }

        public TranslationFileInfo()
        {
            Language = string.Empty;
        }

        public TranslationFileInfo(string language)
        {
            Language = language;
        }
    }
}