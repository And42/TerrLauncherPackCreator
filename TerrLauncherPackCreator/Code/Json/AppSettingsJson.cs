using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AppSettingsJson
    {
        [NotNull]
        [JsonProperty("app_language", Required = Required.Always)]
        public string AppLanguage { get; set; }

        public AppSettingsJson()
        {
            AppLanguage = string.Empty;
        }

        public AppSettingsJson([NotNull] string appLanguage)
        {
            AppLanguage = appLanguage;
        }
    }
}