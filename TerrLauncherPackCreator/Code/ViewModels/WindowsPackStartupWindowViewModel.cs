using System.Windows.Media;
using CommonLibrary;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class WindowsPackStartupWindowViewModel : PackStartupWindowViewModel
    {
        public SolidColorBrush WindowBackground { get; } = new SolidColorBrush(
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once UnreachableCode
            (Color) ColorConverter.ConvertFromString(CommonConstants.IsPreview ? "#ef5350" : "#66bb6a")
        );
        
        public WindowsPackStartupWindowViewModel(
            [NotNull] IAttachedWindowManipulator attachedWindowManipulator,
            [NotNull] AppSettingsJson appSettings
        ) : base(attachedWindowManipulator, appSettings) { }

        protected override void ShowMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void ShowConverterWindow()
        {
            new ConverterWindow().Show();
        }

        protected override void ChooseExistingPackCommand_Execute()
        {
            string filters = $"{StringResources.ChoosePackDialogFilter}|*{PackUtils.PacksExtension};*{PackUtils.PacksActualExtension}";

            var dialog = new OpenFileDialog
            {
                Title = StringResources.ChoosePackDialogTitle,
                Filter = filters,
                CheckFileExists = true
            };
            
            if (dialog.ShowDialog() != true)
            {
                MessageBoxUtils.ShowError(StringResources.ChoosePackDialogFailed);
                return;
            }
            
            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ViewModel.PackProcessor.LoadPackFromFile(dialog.FileName);

            _attachedWindowManipulator.Close();
        }
    }
}