using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new PackStartupWindow().Show();

            Task.Run(async () =>
            {
                try
                {
                    int[] appVersion = UpdateUtils.ConvertVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
                    int[] latestVersion = UpdateUtils.ConvertVersion(await UpdateUtils.GetLatestVersionAsync());

                    for (int i = 0; i < 4; i++)
                    {
                        if (appVersion[i] < latestVersion[i])
                        {
                            Process.Start(Path.Combine(ApplicationDataUtils.PathToRootFolder, "updater.exe"));
                            Environment.Exit(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CrashUtils.HandleException(ex);
                }
            });
        }
    }
}
