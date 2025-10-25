using System.Text.Json.Serialization;
using CrossPlatform.Code.Utils;

namespace TerrLauncherPackCreator.Code.Json;

public class AppSettingsJson
{
    [JsonRequired]
    [JsonPropertyName("app_language")]
    public string AppLanguage { get; set; }
        
    [JsonPropertyName("main_window_width")]
    public int MainWindowWidth { get; set; }

    [JsonPropertyName("main_window_height")]
    public int MainWindowHeight { get; set; }
        
    [JsonPropertyName("pack_structure_version")]
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