using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AppSettingsJson
    {
        [JsonProperty("app_language")]
        public string AppLanguage { get; set; }
    }
}