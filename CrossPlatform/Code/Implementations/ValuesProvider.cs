using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public static class ValuesProvider
    {
        [NotNull]
        public static AppSettingsJson AppSettings => _appSettings ??= AppUtils.LoadAppSettings();
        [CanBeNull]
        private static AppSettingsJson _appSettings;
    }
}