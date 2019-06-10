using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using CommonLibrary.CommonUtils;
using Microsoft.Win32;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using Image = System.Windows.Controls.Image;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackCreationViewModel : BindableBase
    {
        private static readonly ISet<string> IconExtensions = new HashSet<string> {".png", ".gif"};
        private static readonly ISet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif"};
        private readonly IPackProcessor _packProcessor;

        public PackTypes PackType
        {
            get => _packType;
            set => SetProperty(ref _packType, value);
        }

        #region backing fields
        private PackTypes _packType;
        #endregion

        #region Properties

        #region Step 1

        public Uri Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
        public string IconFilePath
        {
            get => _iconFilePath;
            set => SetProperty(ref _iconFilePath, value);
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public string DescriptionRussian
        {
            get => _descriptionRussian;
            set => SetProperty(ref _descriptionRussian, value);
        }
        public string DescriptionEnglish
        {
            get => _descriptionEnglish;
            set => SetProperty(ref _descriptionEnglish, value);
        }
        public Guid Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }
        public int Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        #region backing fields
        private Uri _icon;
        private string _iconFilePath;
        private string _title;
        private string _descriptionRussian;
        private string _descriptionEnglish;
        private Guid _guid;
        private int _version;
        #endregion

        #endregion

        #region Step 2

        public ObservableCollection<PreviewItemModel> Previews
        {
            get => _previews;
            set => SetProperty(ref _previews, value);
        }

        #region backing fields
        private ObservableCollection<PreviewItemModel> _previews;
        #endregion

        #endregion

        #region Step 3

        public ObservableCollection<ModifiedItemModel> ModifiedFiles
        {
            get => _modifiedFiles;
            set => SetProperty(ref _modifiedFiles, value);
        }

        #region backing fields
        private ObservableCollection<ModifiedItemModel> _modifiedFiles;
        #endregion

        #endregion

        #region Step 4

        public string PackFilesExtension
        {
            get => _packFilesExtension;
            set => SetProperty(ref _packFilesExtension, value);
        }

        #region backing fields
        private string _packFilesExtension;
        #endregion

        #endregion

        #endregion

        #region Commands

        #region Step 1

        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<(string filePath, Image iconHolder)> DropIconCommand { get; }

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

        // ReSharper disable once UnusedMember.Global
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

            Guid = Guid.NewGuid();
            Version = 1;
            Previews = new ObservableCollection<PreviewItemModel>();
            ModifiedFiles = new ObservableCollection<ModifiedItemModel>();

            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute);
            DropIconCommand = new ActionCommand<(string filePath, Image iconHolder)>(DropIconCommand_Execute, DropIconCommand_CanExecute);
            DropPreviewsCommand = new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute, DeletePreviewItemCommand_CanExecute);

            DropModifiedFileCommand = new ActionCommand<string[]>(DropModifiedFileCommand_Execute, DropModifiedFileCommand_CanExecute);
            DeleteModifiedItemCommand = new ActionCommand<ModifiedItemModel>(DeleteModifiedItemCommand_Execute, DeleteModifiedItemCommand_CanExecute);

            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute);

            ResetCollections();

            PackFilesExtension = PackUtils.PacksInfo.First(it => it.packType == PackType).packFilesExt;

            _packProcessor.PackLoaded += PackProcessorOnPackLoaded;
            _packProcessor.PackSaved += PackProcessorOnPackSaved;

            PropertyChanged += OnPropertyChanged;
        }

        private void PackProcessorOnPackLoaded((string filePath, PackModel loadedPack, Exception error) item)
        {
            if (item.error == null)
            {
                InitFromPackModel(item.loadedPack);
                return;
            }

            CrashUtils.HandleException(item.error);

            MessageBoxUtils.ShowError(
                string.Format(StringResources.PackLoadingFailed, item.filePath, item.error.Message)
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
                    Process.Start("explorer.exe", $"/select, \"{item.targetFilePath}\"");
                }

                return;
            }

            CrashUtils.HandleException(item.error);

            MessageBoxUtils.ShowError(
                string.Format(StringResources.SavingPackFailed, item.pack.Title, item.error.Message)
            );
        }

        public void InitFromPackModel(PackModel packModel)
        {
            if (packModel == null)
                throw new ArgumentNullException(nameof(packModel));

            ResetCollections();

            PackType = packModel.PackType;
            IconFilePath = packModel.IconFilePath;
            Title = packModel.Title;
            DescriptionRussian = packModel.DescriptionRussian;
            DescriptionEnglish = packModel.DescriptionEnglish;
            Guid = packModel.Guid;
            Version = packModel.Version;

            var previewItems = packModel.PreviewsPaths?.Select(PreviewItemModel.FromImageFile).ToArray();
            var modifiedItems = packModel.ModifiedFilesPaths?.Select(ModifiedItemModel.FromFile).ToArray();

            if (previewItems != null || modifiedItems != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    previewItems?.ForEach(Previews.Add);
                    modifiedItems?.ForEach(ModifiedFiles.Add);
                });
            }

            if (packModel.IconFilePath != null)
            {
                Icon = new Uri(packModel.IconFilePath);
            }
        }
        

        #region Step 1

        private void CreateNewGuidCommand_Execute()
        {
            Guid = Guid.NewGuid();
        }

        private bool DropIconCommand_CanExecute((string filePath, Image iconHolder) parameters)
        {
            return File.Exists(parameters.filePath) && IconExtensions.Contains(Path.GetExtension(parameters.filePath));
        }

        private void DropIconCommand_Execute((string filePath, Image iconHolder) parameters)
        {
            try
            {
                Icon = new Uri(parameters.filePath);
                IconFilePath = parameters.filePath;
            }
            catch (Exception ex)
            {
                MessageBoxUtils.ShowError(
                    string.Format(StringResources.LoadIconFromFileFailed, ex.Message)
                );
            }
        }

        #endregion

        #region Step 2

        private bool DropPreviewsCommand_CanExecute(string[] files)
        {
            return files != null && files.All(it => File.Exists(it) && PreviewExtensions.Contains(Path.GetExtension(it)));
        }

        private void DropPreviewsCommand_Execute(string[] files)
        {
            foreach (string file in files)
            {
                if (Previews.Any(item => item.FilePath == file))
                    continue;

                Previews.Add(new PreviewItemModel(file, false));
            }
        }

        private bool DeletePreviewItemCommand_CanExecute(PreviewItemModel previewItem)
        {
            return !previewItem.IsDragDropTarget;
        }

        private void DeletePreviewItemCommand_Execute(PreviewItemModel previewItem)
        {
            Previews.Remove(previewItem);
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
                if (ModifiedFiles.Any(item => item.FilePath == file))
                    continue;

                ModifiedFiles.Add(new ModifiedItemModel(file, false));
            }
        }

        private bool DeleteModifiedItemCommand_CanExecute(ModifiedItemModel item)
        {
            return !item.IsDragDropTarget;
        }

        private void DeleteModifiedItemCommand_Execute(ModifiedItemModel item)
        {
            ModifiedFiles.Remove(item);
        }

        #endregion

        #region Step 4

        private void ExportPackCommand_Execute()
        {
            var packInfo = PackUtils.PacksInfo.First(it => it.packType == PackType);

            var dialog = new SaveFileDialog
            {
                Title = StringResources.SavePackDialogTitle,
                Filter = $"{packInfo.title} (*{packInfo.packExt})|*{packInfo.packExt}",
                AddExtension = true
            };

            if (dialog.ShowDialog() != true)
            {
                MessageBoxUtils.ShowError(StringResources.SavePackDialogFailed);
                return;
            }

            PackModel packModel = GeneratePackModel();

            _packProcessor.SavePackToFile(packModel, dialog.FileName);
        }

        #endregion

        private PackModel GeneratePackModel()
        {
            return new PackModel
            {
                PackType = PackType,
                IconFilePath = IconFilePath,
                Title = Title,
                DescriptionRussian = DescriptionRussian,
                DescriptionEnglish = DescriptionEnglish,
                Guid = Guid,
                Version = Version,
                PreviewsPaths = Previews.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray(),
                ModifiedFilesPaths = ModifiedFiles.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray()
            };
        }

        private void ResetCollections()
        {
            Previews = new ObservableCollection<PreviewItemModel>();
            ModifiedFiles = new ObservableCollection<ModifiedItemModel>();

            Previews.Add(new PreviewItemModel(filePath: null, isDragDropTarget: true));
            ModifiedFiles.Add(new ModifiedItemModel(null, true));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PackType):
                    PackFilesExtension = PackUtils.PacksInfo.First(it => it.packType == PackType).packFilesExt;
                    break;
            }
        }
    }
}
