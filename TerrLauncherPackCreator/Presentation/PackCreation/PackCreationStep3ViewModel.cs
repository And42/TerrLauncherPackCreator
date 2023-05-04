using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.FileInfos;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Models;
using CrossPlatform.Code.Utils;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

public partial class PackCreationViewModel
{
    public ObservableCollection<ModifiedFilesGroupModel> ModifiedFileGroups { get; } = new();
    
    public IActionCommand<(string[] files, ModifiedFileModel dropModel)> DropModifiedFileCommand { get; private init; } = null!;
    public IActionCommand<ModifiedFileModel> DeleteModifiedItemCommand { get; private init; } = null!;
    public IActionCommand<ModifiedFileModel> SaveResourceCommand { get; private init; } = null!;

    private object? InitializeStep3
    {
        // ReSharper disable once ValueParameterNotUsed
        init
        {
            DropModifiedFileCommand = new ActionCommand<(string[] files, ModifiedFileModel dropModel)>(
                DropModifiedFileCommand_Execute,
                DropModifiedFileCommand_CanExecute
            );
            DeleteModifiedItemCommand = new ActionCommand<ModifiedFileModel>(
                DeleteModifiedItemCommand_Execute,
                DeleteModifiedItemCommand_CanExecute
            );
            SaveResourceCommand = new ActionCommand<ModifiedFileModel>(SaveResourceCommand_Execute, SaveResourceCommand_CanExecute);
            
            ResetStep3Collections();
            
            PropertyChanged += OnPropertyChangedStep3;
        }
    }

    private void InitStep3FromPackModel(PackModel packModel)
    {
        ResetStep3Collections();
        
        foreach (PackModel.ModifiedFile modifiedFile in packModel.ModifiedFiles)
        {
            var fileGroup = ModifiedFileGroups.FirstOrDefault(it => it.FilesType == modifiedFile.FileType);
            fileGroup?.ModifiedFiles.Add(
                FileInfoToModelConverter.Convert(fileGroup.FilesType, modifiedFile.FilePath, modifiedFile.Config)
            );
        }
    }
    
    private void ResetStep3Collections()
    {
        ModifiedFileGroups.Clear();
        
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
            group.ModifiedFiles.Add(
                new ModifiedFileModel(
                    filePath: "drop_target" + initialFilesExt,
                    isDragDropTarget: true
                )
            );
            ModifiedFileGroups.Add(group);
        }
    }
    
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

                fileGroup.ModifiedFiles.Add(FileInfoToModelConverter.Convert(fileGroup.FilesType, file, null));
            }
        }
    }

    private bool DeleteModifiedItemCommand_CanExecute(ModifiedFileModel file)
    {
        return !Working && !file.IsDragDropTarget;
    }

    private void DeleteModifiedItemCommand_Execute(ModifiedFileModel file)
    {
        foreach (ModifiedFilesGroupModel fileGroup in ModifiedFileGroups)
            if (fileGroup.ModifiedFiles.Remove(file))
                return;

        Debug.WriteLine($"Can't delete modified file: {file}");
    }
        
    private bool SaveResourceCommand_CanExecute(ModifiedFileModel file)
    {
        (1 / (8 / FileTypeEnum.Length)).Ignore();

        return !Working && !file.IsDragDropTarget
                        && file is ModifiedTextureModel or ModifiedGuiModel or ModifiedFontModel or ModifiedAudioModel or ModifiedModModel;
    }

    private static void SaveResourceCommand_Execute(ModifiedFileModel file)
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

    private void OnPropertyChangedStep3(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Working):
                DropModifiedFileCommand.RaiseCanExecuteChanged();
                DeleteModifiedItemCommand.RaiseCanExecuteChanged();
                SaveResourceCommand.RaiseCanExecuteChanged();
                break;
        }
    }
    
    private static class FileInfoToModelConverter
    {
        public static ModifiedFileModel Convert(FileType fileType, string filePath, IPackFileInfo? fileInfo)
        {
            (1 / (8 / FileTypeEnum.Length)).Ignore();
            
            return fileType switch
            {
                FileType.Texture => Convert(filePath, (TextureFileInfo?) fileInfo),
                FileType.Gui => Convert(filePath, (GuiFileInfo?) fileInfo),
                FileType.Map => Convert(filePath, (MapFileInfo?) fileInfo),
                FileType.Character => Convert(filePath, (CharacterFileInfo?) fileInfo),
                FileType.Translation => Convert(filePath, (TranslationFileInfo?) fileInfo),
                FileType.Font => Convert(filePath, (FontFileInfo?) fileInfo),
                FileType.Audio => Convert(filePath, (AudioFileInfo?) fileInfo),
                FileType.Mod => Convert(filePath, (ModFileInfo?) fileInfo),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }
        
        private static ModifiedTextureModel Convert(string filePath, TextureFileInfo? fileInfo)
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

        private static ModifiedGuiModel Convert(string filePath, GuiFileInfo? fileInfo)
        {
            var model = new ModifiedGuiModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = null;
                model.Name = fileInfo.EntryName;
            }

            return model;
        }

        private static ModifiedMapModel Convert(string filePath, MapFileInfo? fileInfo)
        {
            var model = new ModifiedMapModel(filePath, false);
            if (fileInfo != null)
            {
                model.ResultFileName = fileInfo.ResultFileName;
            }

            return model;
        }

        private static ModifiedCharacterModel Convert(string filePath, CharacterFileInfo? fileInfo)
        {
            var model = new ModifiedCharacterModel(filePath, false);
            if (fileInfo != null)
            {
                model.ResultFileName = fileInfo.ResultFileName;
            }

            return model;
        }

        private static ModifiedTranslationModel Convert(string filePath, TranslationFileInfo? fileInfo)
        {
            var model = new ModifiedTranslationModel(filePath, false);
            if (fileInfo != null)
            {
                model.CurrentLanguage = fileInfo.Language;
                model.IgnoreForCategory = fileInfo.IgnoreForCategory;
            }

            return model;
        }

        private static ModifiedFontModel Convert(string filePath, FontFileInfo? fileInfo)
        {
            var model = new ModifiedFontModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = null;
                model.Name = fileInfo.EntryName;
            }

            return model;
        }

        private static ModifiedAudioModel Convert(string filePath, AudioFileInfo? fileInfo)
        {
            var model = new ModifiedAudioModel(filePath, false);
            if (fileInfo != null)
            {
                model.Prefix = "";
                model.Name = fileInfo.EntryName;
            }

            return model;
        }
            
        private static ModifiedModModel Convert(string filePath, ModFileInfo? fileInfo)
        {
            var model = new ModifiedModModel(filePath, false);
            if (fileInfo != null)
            {
                model.Id = fileInfo.Id;
                model.IgnoreForCategory = fileInfo.IgnoreForCategory;
            }

            return model;
        }
    }
}