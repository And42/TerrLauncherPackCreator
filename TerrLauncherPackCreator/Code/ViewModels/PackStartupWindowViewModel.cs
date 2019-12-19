using System;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Interfaces;
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

        public PackStartupWindowViewModel([NotNull] IAttachedWindowManipulator attachedWindowManipulator)
        {
            _attachedWindowManipulator = attachedWindowManipulator;

            CreateNewPackCommand = new ActionCommand(CreateNewPackCommand_Execute);
            ChooseExistingPackCommand = new ActionCommand(ChooseExistingPackCommand_Execute);
        }

        private void CreateNewPackCommand_Execute()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            _attachedWindowManipulator.Close();
        }

        private void ChooseExistingPackCommand_Execute()
        {
            // todo: add
            throw new NotSupportedException();
            
//            string allExtsCommaSeparated = string.Join(", ", PackUtils.PacksInfo.Select(it => $"*{it.packExt}"));
//            string allExtsCombined = string.Join(";", PackUtils.PacksInfo.Select(it => $"*{it.packExt}"));
//            string allTypesCombined = string.Join("|", PackUtils.PacksInfo.Select(it => $"{it.title} (*{it.packExt})|*{it.packExt}"));
//
//            string filters = $"{StringResources.ChoosePackDialogFilter} ({allExtsCommaSeparated})|{allExtsCombined}|{allTypesCombined}";
//
//            var dialog = new OpenFileDialog
//            {
//                Title = StringResources.ChoosePackDialogTitle,
//                Filter = filters,
//                CheckFileExists = true
//            };
//
//            if (dialog.ShowDialog() != true)
//            {
//                MessageBoxUtils.ShowError(StringResources.ChoosePackDialogFailed);
//                return;
//            }
//
//            Debug.Assert(PackUtils.PacksInfo.Select(it => it.packExt).Contains(Path.GetExtension(dialog.FileName)));
//
//            var mainWindow = new MainWindow();
//            mainWindow.Show();
//            mainWindow.ViewModel.PackProcessor.LoadPackFromFile(dialog.FileName);
//
//            _attachedWindowManipulator.Close();
//
//            MessageBoxUtils.ShowInformation(StringResources.ChoosePackProcessStarted);
        }
    }
}
