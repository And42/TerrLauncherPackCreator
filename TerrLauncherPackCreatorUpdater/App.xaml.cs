using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;
using TerrLauncherPackCreatorUpdater.Windows;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace TerrLauncherPackCreatorUpdater
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string[] commandLineArguments = e.Args;

            if (commandLineArguments.Length == 0)
            {
                string currentExeLocation = Assembly.GetExecutingAssembly().Location;

                string tempFile = Path.GetTempFileName();

                File.Copy(currentExeLocation, tempFile, true);
                Process.Start(new ProcessStartInfo(tempFile, "process_update")
                {
                    UseShellExecute = false
                });

                Shutdown(0);
                return;
            }

            switch (commandLineArguments[0])
            {
                case "process_update":
                    break;
                case "delete_temp":
                    if (commandLineArguments.Length != 2)
                    {
                        InvalidArguments();
                        return;
                    }

                    try
                    {
                        IOUtils.TryDeleteFile(commandLineArguments[1], 20, 500);
                    }
                    catch (Exception ex)
                    {
                        CrashUtils.HandleException(ex);
                    }

                    Shutdown(0);
                    return;
                default:
                    InvalidArguments();
                    return;
            }

            base.OnStartup(e);

            new UpdaterWindow().Show();
        }

        private void InvalidArguments()
        {
            MessageBox.Show(
                StringResources.InvalidArguments,
                StringResources.ErrorLower,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            Shutdown(1);
        }
    }
}
