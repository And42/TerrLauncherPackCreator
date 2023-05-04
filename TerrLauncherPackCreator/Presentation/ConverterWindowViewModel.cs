using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.FileInfos;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Interfaces;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Presentation;

public class ConverterWindowViewModel : ViewModelBase
{
    public FileType CurrentFileType
    {
        get => _currentFileType;
        set => SetProperty(ref _currentFileType, value);
    }

    public string SourceFilesExtension => PackUtils.GetInitialFilesExt(CurrentFileType);
    public string ConvertedFilesExtension => PackUtils.GetConvertedFilesExt(CurrentFileType);

    public FileType[] FileTypes { get; } = PackUtils.PacksInfo.Select(it => it.FileType).ToArray();

    private readonly IFileConverter _fileConverter = new FileConverter(SessionHelper.Instance);
    private FileType _currentFileType = FileType.Texture;
        
    public IActionCommand<string[]> DropSourceFilesCommand { get; }
    public IActionCommand<string[]> DropConvertedFiledCommand { get; }

    public ConverterWindowViewModel()
    {
        DropSourceFilesCommand = new ActionCommand<string[]>(DropSourceFilesCommand_Execute, DropSourceFilesCommand_CanExecute);
        DropConvertedFiledCommand = new ActionCommand<string[]>(DropConvertedFilesCommand_Execute, DropConvertedFilesCommand_CanExecute);
            
        PropertyChanged += OnPropertyChanged;
    }

    private bool DropSourceFilesCommand_CanExecute(string[] files)
    {
        if (Working)
            return false;

        string currentExt = SourceFilesExtension;
        return files.All(it => Path.GetExtension(it) == currentExt);
    }

    private async void DropSourceFilesCommand_Execute(string[] files)
    {
        using (LaunchWork())
        {
            try
            {
                foreach (string file in files)
                {
                    string resultsDir = Path.Combine(Path.GetDirectoryName(file) ?? string.Empty, "converted_files");
                    IOUtils.EnsureDirExists(resultsDir);
                        
                    var (convertedFile, _) = await _fileConverter.ConvertToTarget(CurrentFileType, file, null);
                    string resultFileExt = PackUtils.GetConvertedFilesExt(CurrentFileType);
                    string resultFile = Path.Combine(resultsDir, Path.GetFileNameWithoutExtension(file) + resultFileExt);
                    File.Copy(convertedFile, resultFile, overwrite: true);
                    File.Delete(convertedFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception: " + ex);
            }
        }
    }

    private bool DropConvertedFilesCommand_CanExecute(string[] files)
    {
        if (Working)
            return false;

        string currentExt = ConvertedFilesExtension;
        return files.All(it => Path.GetExtension(it) == currentExt);
    }

    private async void DropConvertedFilesCommand_Execute(string[] files)
    {
        using (LaunchWork())
        {
            try
            {
                foreach (string file in files)
                {
                    string resultsDir = Path.Combine(Path.GetDirectoryName(file) ?? string.Empty, "source_files");
                    IOUtils.EnsureDirExists(resultsDir);

                    string? config = Path.ChangeExtension(file, PackUtils.PackFileConfigExtension);
                    if (!File.Exists(config))
                        config = null;
        
                    var (convertedFile, fileInfo) = await _fileConverter.ConvertToSource(
                        packStructureVersion: PackUtils.LatestPackStructureVersion,
                        fileType: CurrentFileType,
                        targetFile: file,
                        configFile: config
                    );
                    string resultFileExt = PackUtils.GetInitialFilesExt(CurrentFileType);

                    string resultFileName = Path.GetFileNameWithoutExtension(file);
                    if (fileInfo != null)
                    {
                        (1 / (8 / FileTypeEnum.Length)).Ignore();
#pragma warning disable CS8524
                        resultFileName = CurrentFileType switch
#pragma warning restore CS8524
                        {
                            FileType.Texture => ((TextureFileInfo) fileInfo).EntryName,
                            FileType.Map => ((MapFileInfo) fileInfo).ResultFileName,
                            FileType.Character => ((CharacterFileInfo) fileInfo).ResultFileName,
                            FileType.Gui => ((GuiFileInfo) fileInfo).EntryName,
                            FileType.Translation => ((TranslationFileInfo) fileInfo).Language,
                            FileType.Font => ((FontFileInfo) fileInfo).EntryName,
                            FileType.Audio => ((AudioFileInfo) fileInfo).EntryName,
                            FileType.Mod => resultFileName
                        };
                    }
                        
                    foreach (char invalidChar in Path.GetInvalidFileNameChars())
                        resultFileName = resultFileName.Replace(invalidChar, '_');

                    string resultFile;
                    int index = 1;
                    do
                    {
                        resultFile = Path.Combine(resultsDir, resultFileName + (index == 1 ? "" : "_" + index) + resultFileExt);
                        index++;
                    } while (File.Exists(resultFile));
                        
                    File.Copy(convertedFile, resultFile, overwrite: true);
                    File.Delete(convertedFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown exception: " + ex);
            }
        }
    }
        
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CurrentFileType):
                OnPropertyChanged(nameof(SourceFilesExtension));
                OnPropertyChanged(nameof(ConvertedFilesExtension));
                break;
            case nameof(Working):
                DropSourceFilesCommand.RaiseCanExecuteChanged();
                break;
        }
    }
}