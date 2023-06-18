using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

public partial class PackCreationViewModel
{
    private readonly IPackProcessor _packProcessor = null!;
    private readonly AppSettingsJson _appSettings = null!;
    private readonly Action? _restartApp;
    public IActionCommand ExportPackCommand { get; private init; } = null!;
    public IActionCommand RestartSequenceCommand { get; private init; } = null!;

    private (IPackProcessor packProcessor, AppSettingsJson appSettings, Action? restartApp) InitializeStep5
    {
        // ReSharper disable once ValueParameterNotUsed
        init
        {
            _packProcessor = value.packProcessor;
            _appSettings = value.appSettings;
            _restartApp = value.restartApp;
            
            ExportPackCommand = new ActionCommand(ExportPackCommand_Execute, ExportPackCommand_CanExecute);
            RestartSequenceCommand = new ActionCommand(RestartSequenceCommand_Execute, RestartSequenceCommand_CanExecute);

            _packProcessor.PackSaved += PackProcessorOnPackSaved;

            PropertyChanged += OnPropertyChangedStep5;
        }
    }

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
    
    private static void PackProcessorOnPackSaved((PackModel pack, string targetFilePath, Exception? error) item)
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
    
    private PackModel GeneratePackModel()
    {
        return new PackModel(
            Authors: GeneratePackAuthors(),
            PreviewsPaths: GeneratePackPreviews(),
            ModifiedFiles: GeneratePackModifiedFiles(),
            PackStructureVersion: _appSettings.PackStructureVersion,
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

    private List<PackModel.Author> GeneratePackAuthors()
    {
        return Authors.Select(author =>
            new PackModel.Author(
                Name: author.Name,
                Color: author.Color?.ToDrawingColor(),
                Link: author.Link,
                Icon: author.Image,
                IconHeight: author.IconHeight
            )
        ).ToList();
    }

    private List<string> GeneratePackPreviews()
    {
        return Previews
            .Where(it => !it.IsDragDropTarget)
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
            .ToList();
    }

    private List<PackModel.ModifiedFile> GeneratePackModifiedFiles()
    {
        return ModifiedFileGroups
            .SelectMany(it => it.ModifiedFiles.Select(modified => (it.FilesType, modified)))
            .Where(it => !it.modified.IsDragDropTarget)
            .Select(it =>
            {
                IPackFileInfo fileInfo = ModelToFileInfoConverter.Convert(it.FilesType, it.modified);

                var info = new PackModel.ModifiedFile(
                    Config: fileInfo,
                    FilePath: it.modified.FilePath,
                    FileType: it.FilesType
                );

                return info;
            })
            .ToList();
    }

    private void OnPropertyChangedStep5(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Working):
                ExportPackCommand.RaiseCanExecuteChanged();
                RestartSequenceCommand.RaiseCanExecuteChanged();
                break;
        }
    }
    
    private static class ModelToFileInfoConverter
    {
        public static IPackFileInfo Convert(FileType fileType, ModifiedFileModel model)
        {
            (1 / (8 / FileTypeEnum.Length)).Ignore();
                
            return fileType switch
            {
                FileType.Texture => Convert((ModifiedTextureModel) model),
                FileType.Map => Convert((ModifiedMapModel) model),
                FileType.Character => Convert((ModifiedCharacterModel) model),
                FileType.Gui => Convert((ModifiedGuiModel) model),
                FileType.Translation => Convert((ModifiedTranslationModel) model),
                FileType.Font => Convert((ModifiedFontModel) model),
                FileType.Audio => Convert((ModifiedAudioModel) model),
                FileType.Mod => Convert((ModifiedModModel) model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static AudioFileInfo Convert(ModifiedAudioModel model)
        {
            return new AudioFileInfo(
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name
                    : $"{model.Prefix}/{model.Name}"
            );
        }

        private static TextureFileInfo Convert(ModifiedTextureModel model)
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

        private static MapFileInfo Convert(ModifiedMapModel model)
        {
            return new MapFileInfo(
                ResultFileName: model.ResultFileName ?? string.Empty
            );
        }

        private static CharacterFileInfo Convert(ModifiedCharacterModel model)
        {
            return new CharacterFileInfo(
                ResultFileName: model.ResultFileName
            );
        }

        private static GuiFileInfo Convert(ModifiedGuiModel model)
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

        private static TranslationFileInfo Convert(ModifiedTranslationModel model)
        {
            return new TranslationFileInfo(
                Language: model.CurrentLanguage,
                IgnoreForCategory: model.IgnoreForCategory
            );
        }

        private static FontFileInfo Convert(ModifiedFontModel model)
        {
            return new FontFileInfo(
                EntryName: string.IsNullOrEmpty(model.Prefix)
                    ? model.Name ?? string.Empty
                    : $"{model.Prefix}/{model.Name}"
            );
        }
            
        private static ModFileInfo Convert(ModifiedModModel model)
        {
            return new ModFileInfo(
                Id: model.Id,
                IgnoreForCategory: model.IgnoreForCategory
            );
        }
    }
}