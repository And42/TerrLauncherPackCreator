using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Utils;
using IWshRuntimeLibrary;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;
using TerrLauncherPackCreatorUpdater.Windows;
using File = System.IO.File;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace TerrLauncherPackCreatorUpdater;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        string[] commandLineArguments = e.Args;

        if (commandLineArguments.Length == 0)
        {
            CreateShortcut();
            RunUpdate();
        }

        switch (commandLineArguments[0])
        {
            case "disable_shortcut":
                RunUpdate();
                break;
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

                Process.Start(GetCreatorPath());

                Shutdown(0);
                return;
            default:
                InvalidArguments();
                return;
        }

        base.OnStartup(e);

        new UpdaterWindow().Show();
    }

    private static string GetCreatorPath()
    {
        return Path.Combine(ApplicationDataUtils.PathToRootFolder, "TerrLauncherPackCreator.exe");
    }

    private static void RunUpdate()
    {
        string currentExeLocation = Process.GetCurrentProcess().MainModule!.FileName;

        string tempFile = Path.GetTempFileName();

        File.Copy(currentExeLocation, tempFile, true);
        Process.Start(new ProcessStartInfo(tempFile, "process_update")
        {
            UseShellExecute = false
        });

        Environment.Exit(0);
    }

    private static void CreateShortcut()
    {
        var shell = new WshShell();

        var shortcut = (IWshShortcut) shell.CreateShortcut(
            Path.Combine(AppContext.BaseDirectory, "TerrLauncherPackCreator.lnk")
        );

        shortcut.TargetPath = GetCreatorPath();
        shortcut.Save();
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