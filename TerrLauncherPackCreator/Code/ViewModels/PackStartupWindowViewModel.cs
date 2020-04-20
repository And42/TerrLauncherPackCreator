using System;
using System.Diagnostics;
using System.Linq;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackStartupWindowViewModel : ViewModelBase
    {
        [NotNull]
        private readonly IAttachedWindowManipulator _attachedWindowManipulator;

        [NotNull]
        public IActionCommand CreateNewPackCommand { get; }
        [NotNull]
        public IActionCommand ChooseExistingPackCommand { get; }
        [NotNull]
        public IActionCommand LaunchConverterCommand { get; }

        public PackStartupWindowViewModel([NotNull] IAttachedWindowManipulator attachedWindowManipulator)
        {
            _attachedWindowManipulator = attachedWindowManipulator;

            CreateNewPackCommand = new ActionCommand(CreateNewPackCommand_Execute);
            ChooseExistingPackCommand = new ActionCommand(ChooseExistingPackCommand_Execute);
            LaunchConverterCommand = new ActionCommand(LaunchConverterCommand_Execute);
        }

        private void CreateNewPackCommand_Execute()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            _attachedWindowManipulator.Close();
        }

        private void ChooseExistingPackCommand_Execute()
        {
            string filters = $"{StringResources.ChoosePackDialogFilter}|*{PackUtils.PacksExtension}";

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

        private void LaunchConverterCommand_Execute()
        {
            new ConverterWindow().Show();
            _attachedWindowManipulator.Close();
        }
    }
}
