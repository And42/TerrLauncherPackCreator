using System;
using System.IO;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public static class ValuesProvider
    {
        [NotNull]
        public static AppSettingsJson AppSettings => _appSettings ??= LoadAppSettings();
        [CanBeNull]
        private static AppSettingsJson _appSettings;

        [NotNull]
        private static AppSettingsJson LoadAppSettings()
        {
            if (!File.Exists(Paths.AppSettingsFile))
                return new AppSettingsJson(appLanguage: string.Empty);
            
            try
            {
                return JsonConvert.DeserializeObject<AppSettingsJson>(
                    File.ReadAllText(Paths.AppSettingsFile)
                );
            }
            catch (Exception ex)
            {
                MessageBoxUtils.ShowError($"{StringResources.CantLoadAppSettings} {ex.Message}");
                return new AppSettingsJson(appLanguage: string.Empty);
            }
        }
    }
}