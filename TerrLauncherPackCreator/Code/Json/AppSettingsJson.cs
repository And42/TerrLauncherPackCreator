using CrossPlatform.Code.Utils;
using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json;

public class AppSettingsJson
{
    [JsonProperty("app_language", Required = Required.Always)]
    public string AppLanguage { get; set; }
        
    [JsonProperty("main_window_width")]
    public int MainWindowWidth { get; set; }

    [JsonProperty("main_window_height")]
    public int MainWindowHeight { get; set; }
        
    [JsonProperty("pack_structure_version")]
    public int PackStructureVersion { get; set; }

    public AppSettingsJson()
    {
        AppLanguage = string.Empty;
        MainWindowWidth = 1100;
        MainWindowHeight = 750;
        PackStructureVersion = PackUtils.PackStructureVersions.Latest;
    }

    public AppSettingsJson(
        string appLanguage,
        int mainWindowWidth,
        int mainWindowHeight,
        int packStructureVersion
    )
    {
        AppLanguage = appLanguage;
        MainWindowWidth = mainWindowWidth;
        MainWindowHeight = mainWindowHeight;
        PackStructureVersion = packStructureVersion;
    }
}