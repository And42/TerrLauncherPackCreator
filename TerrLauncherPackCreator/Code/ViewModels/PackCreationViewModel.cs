using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
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
        [NotNull]
        private readonly IPackProcessor _packProcessor;
        [NotNull]
        private readonly string _packTempDir;
        [NotNull]
        private readonly IFileConverter _fileConverter;
        
        #region Properties

        // Step 1
        public int TerrariaStructureVersion
        {
            get => _terrariaStructureVersion;
            set => SetProperty(ref _terrariaStructureVersion, value);
        }

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

        // Step 2
        [NotNull]
        public ObservableCollection<PreviewItemModel> Previews { get; }

        // Step 3
        [NotNull]
        public ObservableCollection<ModifiedFilesGroupModel> ModifiedFileGroups { get; }

        // Step 4
        [NotNull]
        public ObservableCollection<AuthorItemModel> Authors { get; }

        #endregion

        #region Backing fields

        // Step 1
        private int _terrariaStructureVersion = 2;
        private Uri _icon;
        private string _iconFilePath;
        private string _title;
        private string _descriptionRussian;
        private string _descriptionEnglish;
        private Guid _guid;
        private int _version;

        #endregion

        #region Commands

        // Step 1
        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<(string filePath, Image iconHolder)> DropIconCommand { get; }

        // Step 2
        public IActionCommand<string[]> DropPreviewsCommand { get; }
        public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; }

        // Step 3
        public IActionCommand<(string[] files, ModifiedFileModel dropModel)> DropModifiedFileCommand { get; }
        public IActionCommand<ModifiedFileModel> DeleteModifiedItemCommand { get; }

        // Step 5
        public IActionCommand ExportPackCommand { get; }

        #endregion

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable AssignNullToNotNullAttribute
        public PackCreationViewModel() : this(null, null)
        // ReSharper restore AssignNullToNotNullAttribute
        {
            if (!DesignerUtils.IsInDesignMode())
                throw new Exception("This constructor is available only in design mode");
        }

        public PackCreationViewModel(
            [NotNull] IPackProcessor packProcessor,
            [NotNull] ITempDirsProvider tempDirsProvider
        )
        {
            _packProcessor = packProcessor;
            _packTempDir = tempDirsProvider.GetNewDir();
            _fileConverter = new FileConverter(Paths.TextureDefinitionsFile);

            Guid = Guid.NewGuid();
            Version = 1;
            Previews = new ObservableCollection<PreviewItemModel>();
            ModifiedFileGroups = new ObservableCollection<ModifiedFilesGroupModel>();
            Authors = new ObservableCollection<AuthorItemModel>
            {
                // todo: remove
                new AuthorItemModel(),
                new AuthorItemModel()
            };

            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute);
            DropIconCommand = new ActionCommand<(string filePath, Image iconHolder)>(DropIconCommand_Execute, DropIconCommand_CanExecute);
            DropPreviewsCommand = new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute, DeletePreviewItemCommand_CanExecute);

            DropModifiedFileCommand = new ActionCommand<(string[] files, ModifiedFileModel dropModel)>(DropModifiedFileCommand_Execute, DropModifiedFileCommand_CanExecute);
            DeleteModifiedItemCommand = new ActionCommand<ModifiedFileModel>(DeleteModifiedItemCommand_Execute, DeleteModifiedItemCommand_CanExecute);

            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute);

            ResetCollections();

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

            IconFilePath = packModel.IconFilePath;
            Title = packModel.Title;
            DescriptionRussian = packModel.DescriptionRussian;
            DescriptionEnglish = packModel.DescriptionEnglish;
            Guid = packModel.Guid;
            Version = packModel.Version;

            var previewItems = packModel.PreviewsPaths?.Select(PreviewItemModel.FromImageFile).ToArray();
            var modifiedItems = packModel.ModifiedFilesPaths?.Select(ModifiedFileModel.FromFile).ToArray();

            if (previewItems != null || modifiedItems != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    previewItems?.ForEach(Previews.Add);
                    if (modifiedItems == null)
                        return;
                    foreach (var modifiedItem in modifiedItems)
                    {
                        string itemExtension = Path.GetExtension(modifiedItem.FilePath);
                        var fileGroup = ModifiedFileGroups.FirstOrDefault(it => it.FilesExtension == itemExtension);
                        if (fileGroup != null)
                            fileGroup.ModifiedFiles.Add(modifiedItem);
                    }
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

        private bool DropModifiedFileCommand_CanExecute((string[] files, ModifiedFileModel dropModel) parameter)
        {
            return parameter.files != null && parameter.files.All(File.Exists);
        }

        private void DropModifiedFileCommand_Execute((string[] files, ModifiedFileModel dropModel) parameter)
        {
            foreach (string file in parameter.files)
            {
                string fileExtension = Path.GetExtension(file);
                var fileGroup = ModifiedFileGroups.FirstOrDefault(it => it.ModifiedFiles.First() == parameter.dropModel && it.FilesExtension == fileExtension);
                if (fileGroup == null)
                {
                    Debug.WriteLine($"Can't find a group for `{file}` with extension `{fileExtension}`");
                    continue;
                }

                if (fileGroup.ModifiedFiles.Any(item => item.FilePath == file))
                {
                    Debug.WriteLine($"File `{file}` already added; ignoring");
                    continue;
                }

                string convertedFile = _fileConverter.ConvertToTarget(fileGroup.FilesType, file, _packTempDir);
                fileGroup.ModifiedFiles.Add(new ModifiedFileModel(convertedFile, false));
            }
        }

        private bool DeleteModifiedItemCommand_CanExecute(ModifiedFileModel file)
        {
            return !file.IsDragDropTarget;
        }

        private void DeleteModifiedItemCommand_Execute(ModifiedFileModel file)
        {
            foreach (var fileGroup in ModifiedFileGroups)
            {
                if (fileGroup.ModifiedFiles.Remove(file))
                    return;
            }
            
            Debug.WriteLine($"Can't delete modified file: {file}");
        }

        #endregion

        #region Step 4

        private void ExportPackCommand_Execute()
        {
            var dialog = new SaveFileDialog
            {
                Title = StringResources.SavePackDialogTitle,
                Filter = $"{StringResources.TlPacksFilter} (*{PackUtils.PacksExtension})|*{PackUtils.PacksExtension}",
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
                TerrariaStructureVersion = TerrariaStructureVersion,
                IconFilePath = IconFilePath,
                Title = Title,
                DescriptionRussian = DescriptionRussian,
                DescriptionEnglish = DescriptionEnglish,
                Guid = Guid,
                Version = Version,
                PreviewsPaths = Previews.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray(),
                ModifiedFilesPaths = ModifiedFileGroups.SelectMany(it => it.ModifiedFiles).Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray()
            };
        }

        private void ResetCollections()
        {
            Previews.Clear();
            ModifiedFileGroups.Clear();

            Previews.Add(new PreviewItemModel(filePath: null, isDragDropTarget: true));
            foreach ((FileType fileType, string initialFilesExt, string _, string title) in PackUtils.PacksInfo)
            {
                var group = new ModifiedFilesGroupModel(title, initialFilesExt, fileType);
                group.ModifiedFiles.Add(new ModifiedFileModel(filePath: "drop_target" + initialFilesExt, isDragDropTarget: true));
                ModifiedFileGroups.Add(group);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }
    }
}
