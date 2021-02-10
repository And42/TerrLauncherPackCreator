using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AppSettingsJson
    {
        [NotNull]
        [JsonProperty("app_language", Required = Required.Always)]
        public string AppLanguage { get; set; }
        
        [JsonProperty("main_window_width")]
        public int MainWindowWidth { get; set; }

        [JsonProperty("main_window_height")]
        public int MainWindowHeight { get; set; }

        public AppSettingsJson()
        {
            AppLanguage = string.Empty;
            MainWindowWidth = 1100;
            MainWindowHeight = 750;
        }

        public AppSettingsJson(
            [NotNull] string appLanguage,
            int mainWindowWidth,
            int mainWindowHeight
        )
        {
            AppLanguage = appLanguage;
            MainWindowWidth = mainWindowWidth;
            MainWindowHeight = mainWindowHeight;
        }
    }
}