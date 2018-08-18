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

            Task.Run(() =>
            {
                try
                {
                    int[] appVersion = UpdateUtils.ConvertVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
                    int[] latestVersion = UpdateUtils.ConvertVersion(UpdateUtils.GetLatestVersion());

                    for (int i = 0; i < 4; i++)
                    {
                        if (appVersion[i] < latestVersion[i])
                        {
                            Process.Start(
                                Path.Combine(ApplicationDataUtils.PathToRootFolder, "updater.exe"),
                                "disable_shortcut"
                            );
                            Environment.Exit(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CrashUtils.HandleException(ex);
                }
            });

            new PackStartupWindow().Show();
        }
    }
}
