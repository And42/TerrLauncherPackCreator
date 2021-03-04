using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public static class ValuesProvider
    {
        public static AppSettingsJson AppSettings => _appSettings ??= AppUtils.LoadAppSettings();
        private static AppSettingsJson? _appSettings;
    }
}