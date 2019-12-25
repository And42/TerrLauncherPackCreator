using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackCreationViewModel : ViewModelBase
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

        [NotNull]
        public string Log => _log.ToString();

        // Step 4
        [NotNull]
        public ObservableCollection<AuthorItemModel> Authors { get; }

        #endregion

        #region Backing fields

        // Step 1
        private int _terrariaStructureVersion = 2;
        private string _iconFilePath;
        private string _title;
        private string _descriptionRussian;
        private string _descriptionEnglish;
        private Guid _guid;
        private int _version;

        // Step 3
        [NotNull]
        private readonly StringBuilder _log = new StringBuilder();
        
        #endregion

        #region Commands

        // Step 1
        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<string> DropIconCommand { get; }

        // Step 2
        public IActionCommand<string[]> DropPreviewsCommand { get; }
        public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; }

        // Step 3
        public IActionCommand<(string[] files, ModifiedFileModel dropModel)> DropModifiedFileCommand { get; }
        public IActionCommand<ModifiedFileModel> DeleteModifiedItemCommand { get; }

        // Step 4
        public IActionCommand AddAuthorCommand { get; }
        public IActionCommand<AuthorItemModel> DeleteAuthorCommand { get; }
        
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
            _fileConverter = new FileConverter(Paths.TextureDefinitionsFile, WriteToLog);

            Guid = Guid.NewGuid();
            Version = 1;
            Previews = new ObservableCollection<PreviewItemModel>();
            ModifiedFileGroups = new ObservableCollection<ModifiedFilesGroupModel>();
            Authors = new ObservableCollection<AuthorItemModel>();

            // Step 1
            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute, CreateNewGuidCommand_CanExecute);
            DropIconCommand = new ActionCommand<string>(file => {}, DropIconCommand_CanExecute);
            
            // Step 2
            DropPreviewsCommand = new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute, DeletePreviewItemCommand_CanExecute);

            // Step 3
            DropModifiedFileCommand = new ActionCommand<(string[] files, ModifiedFileModel dropModel)>(DropModifiedFileCommand_Execute, DropModifiedFileCommand_CanExecute);
            DeleteModifiedItemCommand = new ActionCommand<ModifiedFileModel>(DeleteModifiedItemCommand_Execute, DeleteModifiedItemCommand_CanExecute);

            // Step 4
            AddAuthorCommand = new ActionCommand(AddAuthorCommand_Execute);
            DeleteAuthorCommand = new ActionCommand<AuthorItemModel>(DeleteAuthorCommand_Execute);
            
            // Step 5
            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute, ExportPackCommand_CanExecute);

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
        }
        

        #region Step 1

        private void CreateNewGuidCommand_Execute()
        {
            Guid = Guid.NewGuid();
        }

        private bool CreateNewGuidCommand_CanExecute()
        {
            return !Working;
        }
        
        private bool DropIconCommand_CanExecute(string filePath)
        {
            return !Working && File.Exists(filePath) && IconExtensions.Contains(Path.GetExtension(filePath));
        }

        #endregion

        #region Step 2

        private bool DropPreviewsCommand_CanExecute(string[] files)
        {
            return !Working && files != null && files.All(it => File.Exists(it) && PreviewExtensions.Contains(Path.GetExtension(it)));
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
            return !Working && !previewItem.IsDragDropTarget;
        }

        private void DeletePreviewItemCommand_Execute(PreviewItemModel previewItem)
        {
            Previews.Remove(previewItem);
        }

        #endregion

        #region Step 3

        private bool DropModifiedFileCommand_CanExecute((string[] files, ModifiedFileModel dropModel) parameter)
        {
            return !Working && parameter.files != null && parameter.files.All(File.Exists);
        }

        private async void DropModifiedFileCommand_Execute((string[] files, ModifiedFileModel dropModel) parameter)
        {
            using (LaunchWork())
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

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    var existingFile = fileGroup.ModifiedFiles.FirstOrDefault(item => Path.GetFileNameWithoutExtension(item.FilePath) == fileName);
                    if (existingFile != null)
                    {
                        Debug.WriteLine($"File `{file}` already added; replacing");
                        SafeFileSystemUtils.DeleteFile(existingFile.FilePath);
                        fileGroup.ModifiedFiles.Remove(existingFile);
                    }

                    try
                    {
                        string convertedFile = await _fileConverter.ConvertToTarget(fileGroup.FilesType, file, _packTempDir);
                        fileGroup.ModifiedFiles.Add(new ModifiedFileModel(convertedFile, false));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(StringResources.ErrorOccurred, ex),
                            StringResources.ErrorLower,
                            MessageBoxButton.OK, MessageBoxImage.Warning
                        );
                    }
                }
            }
        }

        private bool DeleteModifiedItemCommand_CanExecute(ModifiedFileModel file)
        {
            return !Working && !file.IsDragDropTarget;
        }

        private void DeleteModifiedItemCommand_Execute(ModifiedFileModel file)
        {
            foreach (var fileGroup in ModifiedFileGroups)
            {
                SafeFileSystemUtils.DeleteFile(file.FilePath);
                if (fileGroup.ModifiedFiles.Remove(file))
                    return;
            }
            
            Debug.WriteLine($"Can't delete modified file: {file}");
        }

        private void AddAuthorCommand_Execute()
        {
            Authors.Add(new AuthorItemModel());
        }
        
        private void DeleteAuthorCommand_Execute(AuthorItemModel author)
        {
            Authors.Remove(author);
        }
        
        private void WriteToLog([CanBeNull] string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            _log.Append(text);
            if (_log.Length >= 15000)
            {
                int endLineIndex;
                for (endLineIndex = 4000; endLineIndex < 6000 && _log[endLineIndex] != '\n'; endLineIndex++) {}
                _log.Remove(0, endLineIndex + 1);
            }
            OnPropertyChanged(nameof(Log));
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

        private bool ExportPackCommand_CanExecute()
        {
            return !Working;
        }
        
        #endregion

        private PackModel GeneratePackModel()
        {
            return new PackModel(
                Authors.Select(author => (author.Name, author.Color, author.Link, author.ImagePath)).ToArray(),
                Previews.Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray(),
                ModifiedFileGroups.SelectMany(it => it.ModifiedFiles).Where(it => !it.IsDragDropTarget).Select(it => it.FilePath).ToArray()
            )
            {
                TerrariaStructureVersion = TerrariaStructureVersion,
                IconFilePath = IconFilePath,
                Title = Title,
                DescriptionRussian = DescriptionRussian,
                DescriptionEnglish = DescriptionEnglish,
                Guid = Guid,
                Version = Version
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
            switch (e.PropertyName)
            {
                case nameof(Log):
                    break;
                case nameof(Working):
                    // Step 1
                    CreateNewGuidCommand.RaiseCanExecuteChanged();
                    DropIconCommand.RaiseCanExecuteChanged();
                    // Step 2
                    DropPreviewsCommand.RaiseCanExecuteChanged();
                    DeletePreviewItemCommand.RaiseCanExecuteChanged();
                    // Step 3
                    DropModifiedFileCommand.RaiseCanExecuteChanged();
                    DeleteModifiedItemCommand.RaiseCanExecuteChanged();
                    // Step 5
                    ExportPackCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
    }
}
