using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommonLibrary.CommonUtils;
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

        public IProperty<int> PackTypeSelectedIndex { get; }
        public IProperty<ObservableCollection<string>> PackTypeNames { get; }

        public IActionCommand CreateNewPackCommand { get; }
        public IActionCommand ChooseExistingPackCommand { get; }

        public PackStartupWindowViewModel(IAttachedWindowManipulator attachedWindowManipulator)
        {
            _attachedWindowManipulator = attachedWindowManipulator;

            PackTypeSelectedIndex = new FieldProperty<int>(-1);
            PackTypeNames = new FieldProperty<ObservableCollection<string>>
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
                MessageBoxUtils.ShowError(StringResources.PackTypeNotSelected);
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
            string allExtsCommaSeparated = string.Join(", ", PackUtils.PacksInfo.Select(it => $"*{it.packExt}"));
            string allExtsCombined = string.Join(";", PackUtils.PacksInfo.Select(it => $"*{it.packExt}"));
            string allTypesCombined = string.Join("|", PackUtils.PacksInfo.Select(it => $"{it.title} (*{it.packExt})|*{it.packExt}"));

            string filters = $"{StringResources.ChoosePackDialogFilter} ({allExtsCommaSeparated})|{allExtsCombined}|{allTypesCombined}";

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

            Debug.Assert(PackUtils.PacksInfo.Select(it => it.packExt).Contains(Path.GetExtension(dialog.FileName)));

            var mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.ViewModel.PackProcessor.Value.LoadPackFromFile(dialog.FileName);

            _attachedWindowManipulator.Close();

            MessageBoxUtils.ShowInformation(StringResources.ChoosePackProcessStarted);
        }
    }
}
