using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackCreationViewModel : BindableBase
    {
        private readonly IPackProcessor _packProcessor;

        public Property<PackTypes> PackType;

        #region Properties

        #region Step 1

        public Property<BitmapSource> Icon { get; }
        public Property<string> IconFilePath { get; }
        public Property<string> Title { get; }
        public Property<string> DescriptionRussian { get; }
        public Property<string> DescriptionEnglish { get; }
        public Property<Guid> Guid { get; }
        public Property<int> Version { get; }

        #endregion

        #region Step 2

        public Property<ObservableCollection<PreviewItemModel>> Previews { get; }

        #endregion

        #region Step 3

        public Property<ObservableCollection<ModifiedItemModel>> ModifiedFiles { get; }

        #endregion

        #region Step 4

        public Property<string> PackFilesExtension { get; }

        #endregion

        #endregion

        #region Commands

        #region Step 1

        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<string> DropIconCommand { get; }

        #endregion

        #region Step 2

        public IActionCommand<string[]> DropPreviewsCommand { get; }
        public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; }

        #endregion

        #region Step 3

        public IActionCommand<string[]> DropModifiedFileCommand { get; }
        public IActionCommand<ModifiedItemModel> DeleteModifiedItemCommand { get; }

        #endregion

        #region Step 4

        public IActionCommand ExportPackCommand { get; }

        #endregion

        #endregion

        public PackCreationViewModel() : this(null)
        {
            if (!DesignerUtils.IsInDesignMode())
                throw new Exception("This constructor is available only in design mode");
        }

        public PackCreationViewModel(
            IPackProcessor packProcessor
        )
        {
            _packProcessor = packProcessor;

            PackType = new Property<PackTypes>();
            Icon = new Property<BitmapSource>();
            IconFilePath = new Property<string>();
            Title = new Property<string>();
            DescriptionRussian = new Property<string>();
            DescriptionEnglish = new Property<string>();
            Guid = new Property<Guid>(System.Guid.NewGuid());
            Version = new Property<int>(1);
            Previews = new Property<ObservableCollection<PreviewItemModel>>();
            ModifiedFiles = new Property<ObservableCollection<ModifiedItemModel>>();
            PackFilesExtension = new Property<string>();

            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute);
            DropIconCommand = new ActionCommand<string>(DropIconCommand_Execute, DropIconCommand_CanExecute);
            DropPreviewsCommand = new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute, DeletePreviewItemCommand_CanExecute);

            DropModifiedFileCommand = new ActionCommand<string[]>(DropModifiedFileCommand_Execute, DropModifiedFileCommand_CanExecute);
            DeleteModifiedItemCommand = new ActionCommand<ModifiedItemModel>(DeleteModifiedItemCommand_Execute, DeleteModifiedItemCommand_CanExecute);

            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute);

            ResetCollections();

            PackType.PropertyChanged += (sender, args) =>
            {
                PackFilesExtension.Value = PackUtils.PacksInfo.First(it => it.packType == PackType.Value).packFilesExt;
            };

            _packProcessor.PackLoaded += PackProcessorOnPackLoaded;
            _packProcessor.PackSaved += PackProcessorOnPackSaved;
        }

        private void PackProcessorOnPackLoaded((string filePath, PackModel loadedPack, Exception error) item)
        {
            if (item.error == null)
            {
                InitFromPackModel(item.loadedPack);
                return;
            }

            CrashUtils.HandleException(item.error);

            MessageBox.Show(
                string.Format(StringResources.PackLoadingFailed, item.filePath, item.error.Message),
                StringResources.ErrorLower,
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation
            );
        }

        private void PackProcessorOnPackSaved((PackModel pack, string targetFilePath, Exception error) item)
        {
            if (item.error == null)
            {
                MessageBoxResult dialogResult = MessageBox.Show(
                    string.Format(StringResources.PackExported, item.pack.Title),
                    StringResources.InformationLower,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (dialogResult == MessageBoxResult.Yes)
                {
                    Process.Start($"explorer.exe /select, \"{item.targetFilePath}\"");
                }

                return;
            }

            CrashUtils.HandleException(item.error);

            MessageBox.Show(
                string.Format(StringResources.SavingPackFailed, item.pack.Title, item.error.Message),
                StringResources.ErrorLower,
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation
            );
        }

        public void InitFromPackModel(PackModel packModel)
        {
            if (packModel == null)
                throw new ArgumentNullException(nameof(packModel));

            IconFilePath.Value = packModel.IconFilePath;
            
        }

        private PackModel GeneratePackModel()
        {
            return new PackModel
            {
                PackType = PackType.Value,
                IconFilePath = IconFilePath.Value,
                Title = Title.Value,
                DescriptionRussian = DescriptionRussian.Value,
                DescriptionEnglish = DescriptionEnglish.Value,
                Guid = Guid.Value,
                Version = Version.Value,
                PreviewsPaths = Previews.Value.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray(),
                ModifiedFilesPaths = ModifiedFiles.Value.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray()
            };
        }

        private void ResetCollections()
        {
            Previews.Value = new ObservableCollection<PreviewItemModel>();
            ModifiedFiles.Value = new ObservableCollection<ModifiedItemModel>();

            Previews.Value.Add(new PreviewItemModel(null, null, true));
            ModifiedFiles.Value.Add(new ModifiedItemModel(null, true));
        }

        #region Step 1

        private void CreateNewGuidCommand_Execute()
        {
            Guid.Value = System.Guid.NewGuid();
        }

        private bool DropIconCommand_CanExecute(string file)
        {
            return File.Exists(file) && Path.GetExtension(file) == ".png";
        }

        private void DropIconCommand_Execute(string file)
        {
            try
            {
                var bitmap = new Bitmap(file);

                Icon.Value = bitmap.ToBitmapSource();
                IconFilePath.Value = file;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(StringResources.LoadIconFromFileFailed, ex.Message),
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
            }
        }

        #endregion

        #region Step 2

        private bool DropPreviewsCommand_CanExecute(string[] files)
        {
            return files != null && files.All(it => File.Exists(it) && Path.GetExtension(it) == ".jpg");
        }

        private void DropPreviewsCommand_Execute(string[] files)
        {
            foreach (string file in files)
            {
                Bitmap bitmap;

                try
                {
                    bitmap = new Bitmap(file);
                }
                catch (Exception ex)
                {
                    CrashUtils.HandleException(ex);
                    MessageBox.Show(
                        string.Format(StringResources.LoadImageFromFileFailed, file, ex.Message),
                        StringResources.ErrorLower,
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk
                    );
                    continue;
                }

                if (Previews.Value.Any(item => item.FilePath == file))
                    continue;

                Previews.Value.Add(new PreviewItemModel(bitmap.ToBitmapSource(), file, false));
            }
        }

        private bool DeletePreviewItemCommand_CanExecute(PreviewItemModel previewItem)
        {
            return !previewItem.IsDragDropTarget;
        }

        private void DeletePreviewItemCommand_Execute(PreviewItemModel previewItem)
        {
            Previews.Value.Remove(previewItem);
        }

        #endregion

        #region Step 3

        private bool DropModifiedFileCommand_CanExecute(string[] files)
        {
            return files != null && files.All(File.Exists);
        }

        private void DropModifiedFileCommand_Execute(string[] files)
        {
            foreach (string file in files)
            {
                if (ModifiedFiles.Value.Any(item => item.FilePath == file))
                    continue;

                ModifiedFiles.Value.Add(new ModifiedItemModel(file, false));
            }
        }

        private bool DeleteModifiedItemCommand_CanExecute(ModifiedItemModel item)
        {
            return !item.IsDragDropTarget;
        }

        private void DeleteModifiedItemCommand_Execute(ModifiedItemModel item)
        {
            ModifiedFiles.Value.Remove(item);
        }

        #endregion

        #region Step 4

        private void ExportPackCommand_Execute()
        {
            var dialog = new SaveFileDialog
            {
                Title = StringResources.SavePackDialogTitle,
                Filter = StringResources.SavePackDialogFilter + " (*.zip)|*.zip",
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
            {
                MessageBox.Show(
                    StringResources.SavePackDialogFailed,
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
                return;
            }

            PackModel packModel = GeneratePackModel();

            _packProcessor.SavePackToFile(packModel, dialog.FileName);
        }

        #endregion
    }
}
