using System;
using System.IO;
using CommonLibrary.CommonUtils;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class AppUtils
    {
        public static AppSettingsJson LoadAppSettings()
        {
            if (!File.Exists(Paths.AppSettingsFile))
                return new AppSettingsJson();

            try
            {
                return JsonConvert.DeserializeObject<AppSettingsJson>(
                    File.ReadAllText(Paths.AppSettingsFile)
                );
            }
            catch (Exception ex)
            {
                MessageBoxUtils.ShowError($"{StringResources.CantLoadAppSettings} {ex.Message}");
                return new AppSettingsJson();
            }
        }

        public static void SaveAppSettings(AppSettingsJson settings)
        {
            IOUtils.EnsureParentDirExists(Paths.AppSettingsFile);
            File.WriteAllText(
                Paths.AppSettingsFile,
                JsonConvert.SerializeObject(settings, Formatting.Indented)
            );
        }
    }
}