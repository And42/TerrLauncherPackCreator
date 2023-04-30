using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.FileInfos;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Models;
using CrossPlatform.Code.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackCreationViewModel : ViewModelBase
    {
        private static readonly ISet<string> IconExtensions = new HashSet<string> {".png", ".gif"};
        private static readonly ISet<string> PreviewExtensions = new HashSet<string> {".jpg", ".png", ".gif"};
        // ReSharper disable once UnusedMember.Local
        private const int _ = 1 / (32 / PredefinedPackTagEnum.Length);
        private static readonly IReadOnlyList<PredefinedPackTag> AllPredefinedTags = new []
        {
            PredefinedPackTag.TexturesAnimated,
            PredefinedPackTag.TexturesWeapons,
            PredefinedPackTag.TexturesTools,
            PredefinedPackTag.TexturesVanity,
            PredefinedPackTag.TexturesArmor,
            PredefinedPackTag.TexturesPets,
            PredefinedPackTag.TexturesBosses,
            PredefinedPackTag.TexturesMobs,
            PredefinedPackTag.TexturesNpc,
            PredefinedPackTag.TexturesBlocks,
            PredefinedPackTag.TexturesOther,
            PredefinedPackTag.MapsBuildings,
            PredefinedPackTag.MapsAdventure,
            PredefinedPackTag.MapsSurvival,
            PredefinedPackTag.MapsOther,
            PredefinedPackTag.MapsParkour,
            PredefinedPackTag.MapsForMultiplePlayers,
            PredefinedPackTag.CharactersCombat,
            PredefinedPackTag.CharactersAppearance,
            PredefinedPackTag.CharactersOther,
            PredefinedPackTag.GuiAnimated,
            PredefinedPackTag.GuiInventory,
            PredefinedPackTag.GuiHealthOrMana,
            PredefinedPackTag.GuiGeneral,
            PredefinedPackTag.GuiOther,
            PredefinedPackTag.AudioBiomsOrLocation,
            PredefinedPackTag.AudioBosses,
            PredefinedPackTag.AudioEvents,
            PredefinedPackTag.AudioSounds,
            PredefinedPackTag.AudioOther,
            PredefinedPackTag.FontsAnimated,
            PredefinedPackTag.ModsScripts
        };

        private readonly IPackProcessor _packProcessor;

        #region Properties

        // Step 1
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

        public ObservableCollection<PredefinedPackTag> PredefinedTags { get; }

        public IReadOnlyList<PredefinedPackTag> RemainedPredefinedTags => AllPredefinedTags.Except(PredefinedTags).ToList();
        
        public bool IsPredefinedTagsPopupOpen
        {
            get => _isPredefinedTagsPopupOpen;
            set => SetProperty(ref _isPredefinedTagsPopupOpen, value);
        }

        public bool IsBonusPack
        {
            get => _isBonusPack;
            set => SetProperty(ref _isBonusPack, value);
        }

        // Step 2
        public ObservableCollection<PreviewItemModel> Previews { get; }

        // Step 3
        public ObservableCollection<ModifiedFilesGroupModel> ModifiedFileGroups { get; }

        // Step 4
        public ObservableCollection<AuthorItemModel> Authors { get; }

        #endregion

        #region Backing fields

        // Step 1
        private string _iconFilePath = string.Empty;
        private string _title = string.Empty;
        private string _descriptionRussian = string.Empty;
        private string _descriptionEnglish = string.Empty;
        private Guid _guid;
        private int _version;
        private bool _isPredefinedTagsPopupOpen;
        private bool _isBonusPack;

        #endregion

        #region Commands

        // Step 1
        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<string> DropIconCommand { get; }
        public IActionCommand AddPredefinedTagCommand { get; }
        public IActionCommand<PredefinedPackTag> AddSelectedTagCommand { get; }
        public IActionCommand<PredefinedPackTag> RemovePredefinedTag { get; }

        // Step 2
        public IActionCommand<string[]> DropPreviewsCommand { get; }
        public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; }

        // Step 3
        public IActionCommand<(string[] files, ModifiedFileModel dropModel)> DropModifiedFileCommand { get; }
        public IActionCommand<ModifiedFileModel> DeleteModifiedItemCommand { get; }
        public IActionCommand<ModifiedFileModel> SaveResourceCommand { get; }

        // Step 4
        public IActionCommand AddAuthorCommand { get; }
        public IActionCommand<AuthorItemModel> DeleteAuthorCommand { get; }

        // Step 5
        private readonly Action? _restartApp;
        public IActionCommand ExportPackCommand { get; }
        public IActionCommand RestartSequenceCommand { get; }

        #endregion

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable AssignNullToNotNullAttribute
        public PackCreationViewModel() : this(null!, null)
        // ReSharper restore AssignNullToNotNullAttribute
        {
            if (!DesignerUtils.IsInDesignMode())
                throw new Exception("This constructor is available only in design mode");
        }

        public PackCreationViewModel(
            IPackProcessor packProcessor,
            Action? restartApp
        )
        {
            _packProcessor = packProcessor;
            _restartApp = restartApp;

            Guid = Guid.NewGuid();
            Version = 1;
            Previews = new ObservableCollection<PreviewItemModel>();
            ModifiedFileGroups = new ObservableCollection<ModifiedFilesGroupModel>();
            Authors = new ObservableCollection<AuthorItemModel>();

            // Step 1
            PredefinedTags = new ObservableCollection<PredefinedPackTag>();
            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute, CreateNewGuidCommand_CanExecute);
            DropIconCommand = new ActionCommand<string>(_ => { }, DropIconCommand_CanExecute);
            AddPredefinedTagCommand = new ActionCommand(AddPredefinedTagExecuted, AddPredefinedTagCanExecute);
            AddSelectedTagCommand = new ActionCommand<PredefinedPackTag>(AddSelectedTagExecuted);
            RemovePredefinedTag = new ActionCommand<PredefinedPackTag>(RemovePredefinedTagExecuted, RemovePredefinedTagCanExecute);

            // Step 2
            DropPreviewsCommand =
                new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute,
                DeletePreviewItemCommand_CanExecute);

            // Step 3
            DropModifiedFileCommand =
                new ActionCommand<(string[] files, ModifiedFileModel dropModel)>(DropModifiedFileCommand_Execute,
                    DropModifiedFileCommand_CanExecute);
            DeleteModifiedItemCommand = new ActionCommand<ModifiedFileModel>(DeleteModifiedItemCommand_Execute,
                DeleteModifiedItemCommand_CanExecute);
            SaveResourceCommand = new ActionCommand<ModifiedFileModel>(SaveResourceCommand_Execute, SaveResourceCommand_CanExecute);

            // Step 4
            AddAuthorCommand = new ActionCommand(AddAuthorCommand_Execute);
            DeleteAuthorCommand = new ActionCommand<AuthorItemModel>(DeleteAuthorCommand_Execute);

            // Step 5
            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute, ExportPackCommand_CanExecute);
            RestartSequenceCommand = new ActionCommand(RestartSequenceCommand_Execute, RestartSequenceCommand_CanExecute);

            ResetCollections();

            _packProcessor.PackLoaded += PackProcessorOnPackLoaded;
            _packProcessor.PackSaved += PackProcessorOnPackSaved;

            PredefinedTags.CollectionChanged += PredefinedTagsCollectionChanged;
            PropertyChanged += OnPropertyChanged;
        }

        private void PackProcessorOnPackLoaded((string filePath, PackModel? loadedPack, Exception? error) item)
        {
            if (item.error == null)
            {
                InitFromPackModel(item.loadedPack.AssertNotNull());
                return;
            }

            CrashUtils.HandleException(item.error);

            MessageBoxUtils.ShowError(
                string.Format(StringResources.PackLoadingFailed, item.filePath, item.error.Message)
            );
        }

        private void PackProcessorOnPackSaved((PackModel pack, string targetFilePath, Exception? error) item)
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

            MessageBoxUtils.ShowError(
                string.Format(StringResources.SavingPackFailed, item.pack.Title, item.error.Message)
            );
            
            CrashUtils.HandleException(item.error);
        }

        public void InitFromPackModel(PackModel packModel)
        {
            if (packModel == null)
                throw new ArgumentNullException(nameof(packModel));

            IconFilePath = packModel.IconFilePath;
            Title = packModel.Title;
            DescriptionRussian = packModel.DescriptionRussian;
            DescriptionEnglish = packModel.DescriptionEnglish;
            Guid = packModel.Guid;
            Version = packModel.Version;
            IsBonusPack = packModel.IsBonusPack;

            var previewItems = packModel.PreviewsPaths.Select(PreviewItemModel.FromImageFile).ToArray();

            Application.Current.Dispatcher.Invoke(() =>
            {
                ResetCollections();

                foreach (var author in packModel.Authors)
                {
                    Authors.Add(
                        new AuthorItemModel(
                            name: author.Name,
                            color: author.Color?.ToMediaColor(),
                            image: author.Icon,
                            link: author.Link,
                            iconHeight: author.IconHeight
                        )
                    );
                }

                previewItems.ForEach(Previews.Add);
                foreach (var modifiedFile in packModel.ModifiedFiles)
                {
                    var fileGroup = ModifiedFileGroups.FirstOrDefault(it => it.FilesType == modifiedFile.FileType);
                    if (fileGroup != null)
                    {
                        fileGroup.ModifiedFiles.Add(FileToModel(fileGroup.FilesType, modifiedFile.FilePath, modifiedFile.Config));
                    }
                }

                packModel.PredefinedTags.ForEach(PredefinedTags.Add);
            });
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
        
        private bool AddPredefinedTagCanExecute()
        {
            return !Working && RemainedPredefinedTags.Count != 0;
        }

        private void AddPredefinedTagExecuted()
        {
            IsPredefinedTagsPopupOpen = !IsPredefinedTagsPopupOpen;
        }

        private void AddSelectedTagExecuted(PredefinedPackTag tag)
        {
            PredefinedTags.Add(tag);
            IsPredefinedTagsPopupOpen = false;
        }
        
        private bool RemovePredefinedTagCanExecute(PredefinedPackTag _)
        {
            return !Working;
        }

        private void RemovePredefinedTagExecuted(PredefinedPackTag tag)
        {
            PredefinedTags.Remove(tag);
        }


        #endregion

        #region Step 2

        private bool DropPreviewsCommand_CanExecute(string[] files)
        {
            return !Working && files != null &&
                   files.All(it => File.Exists(it) && PreviewExtensions.Contains(Path.GetExtension(it)));
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

        private bool DropModifiedFileCommand_CanExecute((string[]? files, ModifiedFileModel dropModel) parameter)
        {
            return !Working && parameter.files != null && parameter.files.All(File.Exists);
        }

        private void DropModifiedFileCommand_Execute((string[] files, ModifiedFileModel dropModel) parameter)
        {
            using (LaunchWork())
            {
                foreach (string file in parameter.files)
                {
                    string fileExtension = Path.GetExtension(file);
                    var fileGroup = ModifiedFileGroups.FirstOrDefault(it =>
                        it.ModifiedFiles.First() == parameter.dropModel && it.FilesExtension == fileExtension);
                    if (fileGroup == null)
                    {
                        Debug.WriteLine($"Can't find a group for `{file}` with extension `{fileExtension}`");
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    var existingFile = fileGroup.ModifiedFiles.FirstOrDefault(item =>
                        Path.GetFileNameWithoutExtension(item.FilePath) == fileName);
                    if (existingFile != null)
                    {
                        Debug.WriteLine($"File `{file}` already added; replacing");
                        fileGroup.ModifiedFiles.Remove(existingFile);
                    }

                    fileGroup.ModifiedFiles.Add(FileToModel(fileGroup.FilesType, file, null));
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
                if (fileGroup.ModifiedFiles.Remove(file))
                    return;
            }

            Debug.WriteLine($"Can't delete modified file: {file}");
        }
        
        private bool SaveResourceCommand_CanExecute(ModifiedFileModel file)
        {
            (1 / (8 / FileTypeEnum.Length)).Ignore();

            return !Working && !file.IsDragDropTarget
                && file is ModifiedTextureModel or ModifiedGuiModel or ModifiedFontModel or ModifiedAudioModel or ModifiedModModel;
        }

        private void SaveResourceCommand_Execute(ModifiedFileModel file)
        {
            if (file.FilePath.IsNullOrEmpty())
                return;

            (1 / (8 / FileTypeEnum.Length)).Ignore();
            
            void SaveFile(FileType type) => SaveFileResource(extension: PackUtils.GetInitialFilesExt(type), file);
            void SaveFolder() => SaveFolderResource(file);
            
            switch (file)
            {
                case ModifiedTextureModel:
                    SaveFile(type: FileType.Texture);
                    break;
                case ModifiedGuiModel:
                    SaveFile(type: FileType.Gui);
                    break;
                case ModifiedFontModel:
                    SaveFile(type: FileType.Font);
                    break;
                case ModifiedAudioModel:
                    SaveFile(type: FileType.Audio);
                    break;
                case ModifiedModModel:
                    // todo: validate
                    SaveFolder();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(file));
            }
        }

        private static void SaveFileResource(string extension, ModifiedFileModel fileModel)
        {
            if (!File.Exists(fileModel.FilePath))
                return;
            
            var dialog = new SaveFileDialog
            {
                Filter = $"{StringResources.SaveFileFilterTitle} (*{extension})|*{extension}"
            };
            if (dialog.ShowDialog() == true)
                IOUtils.CopyFile(fileModel.FilePath, dialog.FileName, overwrite: true);
        }
        
        private static void SaveFolderResource(ModifiedFileModel fileModel)
        {
            if (!Directory.Exists(fileModel.FilePath))
                return;
            
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                IOUtils.CopyDirectory(fileModel.FilePath, dialog.FileName, overwriteFiles: true);
        }

        private void AddAuthorCommand_Execute()
        {
            Authors.Add(new AuthorItemModel());
        }

        private void DeleteAuthorCommand_Execute(AuthorItemModel author)
        {
            Authors.Remove(author);
        }

        #endregion

        #region Step 4

        private void ExportPackCommand_Execute()
        {
            string fileName = Title;
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(invalidChar, '_');

            var dialog = new SaveFileDialog
            {
                Title = StringResources.SavePackDialogTitle,
                Filter = $"{StringResources.TlPacksFilter} (*{PackUtils.PacksExtension})|*{PackUtils.PacksExtension}",
                AddExtension = true,
                FileName = $"{fileName}.{PackUtils.PacksExtension}"
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

        private void RestartSequenceCommand_Execute()
        {
            var result = MessageBox.Show(
                StringResources.RestartSequenceConfirmation,
                StringResources.InformationLower,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes)
                return;
            
            _restartApp?.Invoke();
        }
        
        private bool RestartSequenceCommand_CanExecute()
        {
            return !Working;
        }

        #endregion

        private PackModel GeneratePackModel()
        {
            return new(
                Authors: Authors.Select(author => 
                    new PackModel.Author(
                        Name: author.Name,
                        Color: author.Color?.ToDrawingColor(),
                        Link: author.Link,
                        Icon: author.Image,
                        IconHeight: author.IconHeight
                    )
                ).ToArray(),
                PreviewsPaths: Previews.Where(it => !it.IsDragDropTarget)
                    .Select(it =>
                    {
                        if (string.IsNullOrEmpty(it.FilePath))
                            throw new ArgumentException();
                        if (!it.IsCroppingAvailable || !it.IsCroppingEnabled)
                            return it.FilePath;

                        Bitmap cropped;
                        using (var bmp = new Bitmap(it.FilePath))
                        {
                            var crop = new Int32Rect(
                                it.CropLeftPixels,
                                it.CropTopPixels,
                                bmp.Width - it.CropLeftPixels - it.CropRightPixels,
                                bmp.Height - it.CropTopPixels - it.CropBottomPixels
                            );
                            cropped = new CroppedBitmap(bmp.ToBitmapSource(), crop).ToBitmap();
                        }

                        using (cropped)
                        {
                            string tempFile = ApplicationDataUtils.GenerateNonExistentFilePath(extension: ".png");
                            cropped.Save(tempFile, ImageFormat.Png);
                            return tempFile;
                        }
                    })
                    .ToArray(),
                ModifiedFiles: ModifiedFileGroups.SelectMany(it => it.ModifiedFiles.Select(modified => (it.FilesType, modified)))
                    .Where(it => !it.modified.IsDragDropTarget)
                    .Select(it =>
                    {
                        IPackFileInfo fileInfo = ModelToFileInfo(it.FilesType, it.modified);

                        var info = new PackModel.ModifiedFile(
                            Config: fileInfo,
                            FilePath: it.modified.FilePath,
                            FileType: it.FilesType
                        );

                        return info;
                    })
                    .ToArray(),
                PackStructureVersion: PackUtils.LatestPackStructureVersion,
                IconFilePath: IconFilePath,
                Title: Title,
                DescriptionRussian: DescriptionRussian,
                DescriptionEnglish: DescriptionEnglish,
                Guid: Guid,
                Version: Version,
                IsBonusPack: IsBonusPack,
                PredefinedTags: PredefinedTags.ToList()
            );
        }

        private static IPackFileInfo ModelToFileInfo(FileType fileType, ModifiedFileModel model)
        {
            (1 / (8 / FileTypeEnum.Length)).Ignore();
            
            return fileType switch
            {
                FileType.Texture => ModelToFileInfo((ModifiedTextureModel) model),
                FileType.Map => ModelToFileInfo((ModifiedMapModel) model),
                FileType.Character => ModelToFileInfo((ModifiedCharacterModel) model),
                FileType.Gui => ModelToFileInfo((ModifiedGuiModel) model),
                FileType.Translation => ModelToFileInfo((ModifiedTranslationModel) model),
                FileType.Font => ModelToFileInfo((ModifiedFontModel) model),
                FileType.Audio => ModelToFileInfo((ModifiedAudioModel) model),
                FileType.Mod => ModelToFileInfo((ModifiedModModel) model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static AudioFileInfo ModelToFileInfo(ModifiedAudioModel model)
        {
            return new AudioFileInfo(
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name
                    : $"{model.Prefix}/{model.Name}"
            );
        }

        private static TextureFileInfo ModelToFileInfo(ModifiedTextureModel model)
        {
            return new TextureFileInfo(
                Type: model.CurrentTextureType,
                Animated: model.Animated,
                AnimateInGui: model.AnimateInGui,
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name ?? string.Empty
                    : $"{model.Prefix}/{model.Name}",
                ElementId: model.ElementId,
                MillisecondsPerFrame: model.MillisecondsPerFrame,
                NumberOfVerticalFrames: model.NumberOfVerticalFrames,
                NumberOfHorizontalFrames: model.NumberOfHorizontalFrames,
                ApplyOriginalSize: model.ApplyOriginalSize
            );
        }

        private static MapFileInfo ModelToFileInfo(ModifiedMapModel model)
        {
            return new MapFileInfo(
                ResultFileName: model.ResultFileName ?? string.Empty
            );
        }

        private static CharacterFileInfo ModelToFileInfo(ModifiedCharacterModel model)
        {
            return new CharacterFileInfo(
                ResultFileName: model.ResultFileName
            );
        }

        private static GuiFileInfo ModelToFileInfo(ModifiedGuiModel model)
        {
            return new GuiFileInfo(
                Type: TextureFileInfo.TextureType.General,
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name ?? string.Empty
                    : $"{model.Prefix}/{model.Name}",
                ElementId: 0,
                Animated: false,
                AnimateInGui: false,
                NumberOfVerticalFrames: 1,
                NumberOfHorizontalFrames: 1,
                MillisecondsPerFrame: 0,
                ApplyOriginalSize: true
            );
        }

        private static TranslationFileInfo ModelToFileInfo(ModifiedTranslationModel model)
        {
            return new TranslationFileInfo(
                Language: model.CurrentLanguage,
                IgnoreForCategory: model.IgnoreForCategory
            );
        }

        private static FontFileInfo ModelToFileInfo(ModifiedFontModel model)
        {
            return new FontFileInfo(
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name ?? string.Empty
                    : $"{model.Prefix}/{model.Name}"
            );
        }
        
        private static ModFileInfo ModelToFileInfo(ModifiedModModel model)
        {
            return new ModFileInfo(
                Id: model.Id,
                IgnoreForCategory: model.IgnoreForCategory
            );
        }

        private static ModifiedFileModel FileToModel(FileType fileType, string filePath, IPackFileInfo? fileInfo)
        {
            (1 / (8 / FileTypeEnum.Length)).Ignore();
            
            return fileType switch
            {
                FileType.Texture => FileInfoToModel(filePath, (TextureFileInfo?) fileInfo),
                FileType.Gui => FileInfoToModel(filePath, (GuiFileInfo?) fileInfo),
                FileType.Map => FileInfoToModel(filePath, (MapFileInfo?) fileInfo),
                FileType.Character => FileInfoToModel(filePath, (CharacterFileInfo?) fileInfo),
                FileType.Translation => FileInfoToModel(filePath, (TranslationFileInfo?) fileInfo),
                FileType.Font => FileInfoToModel(filePath, (FontFileInfo?) fileInfo),
                FileType.Audio => FileInfoToModel(filePath, (AudioFileInfo?) fileInfo),
                FileType.Mod => FileInfoToModel(filePath, (ModFileInfo?) fileInfo),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

        private static ModifiedTextureModel FileInfoToModel(string filePath, TextureFileInfo? fileInfo)
        {
            var model = new ModifiedTextureModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = null;
                model.Name = fileInfo.EntryName;
                model.CurrentTextureType = fileInfo.Type;
                model.Animated = fileInfo.Animated;
                model.AnimateInGui = fileInfo.AnimateInGui;
                model.ElementId = fileInfo.ElementId;
                model.NumberOfVerticalFrames = fileInfo.NumberOfVerticalFrames;
                model.NumberOfHorizontalFrames = fileInfo.NumberOfHorizontalFrames;
                model.MillisecondsPerFrame = fileInfo.MillisecondsPerFrame;
                model.ApplyOriginalSize = fileInfo.ApplyOriginalSize;
            }

            return model;
        }

        private static ModifiedGuiModel FileInfoToModel(string filePath, GuiFileInfo? fileInfo)
        {
            var model = new ModifiedGuiModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = null;
                model.Name = fileInfo.EntryName;
            }

            return model;
        }

        private static ModifiedMapModel FileInfoToModel(string filePath, MapFileInfo? fileInfo)
        {
            var model = new ModifiedMapModel(filePath, false);
            if (fileInfo != null)
            {
                model.ResultFileName = fileInfo.ResultFileName;
            }

            return model;
        }

        private static ModifiedCharacterModel FileInfoToModel(string filePath, CharacterFileInfo? fileInfo)
        {
            var model = new ModifiedCharacterModel(filePath, false);
            if (fileInfo != null)
            {
                model.ResultFileName = fileInfo.ResultFileName;
            }

            return model;
        }

        private static ModifiedTranslationModel FileInfoToModel(string filePath, TranslationFileInfo? fileInfo)
        {
            var model = new ModifiedTranslationModel(filePath, false);
            if (fileInfo != null)
            {
                model.CurrentLanguage = fileInfo.Language;
                model.IgnoreForCategory = fileInfo.IgnoreForCategory;
            }

            return model;
        }

        private static ModifiedFontModel FileInfoToModel(string filePath, FontFileInfo? fileInfo)
        {
            var model = new ModifiedFontModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = null;
                model.Name = fileInfo.EntryName;
            }

            return model;
        }

        private static ModifiedAudioModel FileInfoToModel(string filePath, AudioFileInfo? fileInfo)
        {
            var model = new ModifiedAudioModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = "";
                model.Name = fileInfo.EntryName;
            }

            return model;
        }
        
        private static ModifiedModModel FileInfoToModel(string filePath, ModFileInfo? fileInfo)
        {
            var model = new ModifiedModModel(filePath, false);
            if (fileInfo != null)
            {
                model.Id = fileInfo.Id;
                model.IgnoreForCategory = fileInfo.IgnoreForCategory;
            }

            return model;
        }
        
        private void ResetCollections()
        {
            Previews.Clear();
            ModifiedFileGroups.Clear();
            Authors.Clear();
            PredefinedTags.Clear();

            Previews.Add(new PreviewItemModel(filePath: null, isDragDropTarget: true));
            foreach ((FileType fileType, string initialFilesExt, string _) in PackUtils.PacksInfo)
            {
                (1 / (8 / FileTypeEnum.Length)).Ignore();
                
                string title = fileType switch {
                    FileType.Texture => StringResources.PackTypeTextures,
                    FileType.Map => StringResources.PackTypeMaps,
                    FileType.Character => StringResources.PackTypeCharacters,
                    FileType.Gui => StringResources.PackTypeGui,
                    FileType.Translation => StringResources.PackTypeTranslations,
                    FileType.Font => StringResources.PackTypeFonts,
                    FileType.Audio => StringResources.PackTypeAudio,
                    FileType.Mod => StringResources.PackTypeMods,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var group = new ModifiedFilesGroupModel(title, initialFilesExt, fileType);
                group.ModifiedFiles.Add(new ModifiedFileModel(filePath: "drop_target" + initialFilesExt,
                    isDragDropTarget: true));
                ModifiedFileGroups.Add(group);
            }
        }

        private void PredefinedTagsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(RemainedPredefinedTags));
        }
        
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
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
                    SaveResourceCommand.RaiseCanExecuteChanged();
                    // Step 5
                    ExportPackCommand.RaiseCanExecuteChanged();
                    RestartSequenceCommand.RaiseCanExecuteChanged();
                    break;
                case nameof(RemainedPredefinedTags):
                    AddPredefinedTagCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
    }
}