using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using TerrLauncherPackCreator.Windows;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackStartupWindowViewModel
    {
        private readonly IAttachedWindowManipulator _attachedWindowManipulator;

        public Property<int> PackTypeSelectedIndex { get; }
        public Property<ObservableCollection<string>> PackTypeNames { get; }

        public IActionCommand CreateNewPackCommand { get; }
        public IActionCommand ChooseExistingPackCommand { get; }

        public PackStartupWindowViewModel(IAttachedWindowManipulator attachedWindowManipulator)
        {
            _attachedWindowManipulator = attachedWindowManipulator;

            PackTypeSelectedIndex = new Property<int>(-1);
            PackTypeNames = new Property<ObservableCollection<string>>
            {
                Value = new ObservableCollection<string>(PackUtils.PacksInfo.Select(it => it.title))
            };

            CreateNewPackCommand = new ActionCommand(CreateNewPackCommand_Execute);
            ChooseExistingPackCommand = new ActionCommand(ChooseExistingPackCommand_Execute);
        }

        private void CreateNewPackCommand_Execute()
        {
            if (PackTypeSelectedIndex.Value == -1)
            {
                MessageBox.Show(
                    StringResources.PackTypeNotSelected,
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
                return;
            }

            PackTypes packType = PackUtils.PacksInfo[PackTypeSelectedIndex.Value].packType;

            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ViewModel.PackCreationViewModel.Value.PackType.Value = packType;

            _attachedWindowManipulator.Close();
        }

        private void ChooseExistingPackCommand_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Title = StringResources.ChoosePackDialogTitle,
                Filter = StringResources.ChoosePackDialogFilter + " (*.zip)|*.zip",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() != true)
            {
                MessageBox.Show(
                    StringResources.ChoosePackDialogFailed,
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ViewModel.PackProcessor.Value.LoadPackFromFile(dialog.FileName);

            _attachedWindowManipulator.Close();

            MessageBox.Show(
                StringResources.ChoosePackProcessStarted,
                StringResources.InformationLower,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
