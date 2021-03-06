﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (_, args) =>
            {
                MessageBoxUtils.ShowError($"Unhandled exception: `{args.Exception}`");
            };

            Task.Run(() =>
            {
                try
                {
                    int[] appVersion = UpdateUtils.ConvertVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
                    int[] latestVersion = UpdateUtils.ConvertVersion(UpdateUtils.GetLatestVersion());

                    for (int i = 0; i < 4; i++)
                    {
                        if (appVersion[i] > latestVersion[i])
                            break;
                        if (appVersion[i] == latestVersion[i])
                            continue;

                        Process.Start(
                            Path.Combine(ApplicationDataUtils.PathToRootFolder, "updater.exe"),
                            "disable_shortcut"
                        );
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    CrashUtils.HandleException(ex);
                }
            });

            SetCurrentLanguage();
            new PackStartupWindow().Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            
            if (Directory.Exists(ApplicationDataUtils.PathToSessionTempFolder))
                Directory.Delete(ApplicationDataUtils.PathToSessionTempFolder, true);
        }

        private static void SetCurrentLanguage()
        {
            string appLanguage = ValuesProvider.AppSettings.AppLanguage;
            if (!string.IsNullOrEmpty(appLanguage))
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(appLanguage);
        }
    }
}
