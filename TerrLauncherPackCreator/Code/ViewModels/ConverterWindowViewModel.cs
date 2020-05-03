using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class ConverterWindowViewModel : ViewModelBase
    {
        public FileType CurrentFileType
        {
            get => _currentFileType;
            set => SetProperty(ref _currentFileType, value);
        }

        [NotNull]
        public string SourceFilesExtension => PackUtils.GetInitialFilesExt(CurrentFileType);
        [NotNull]
        public string ConvertedFilesExtension => PackUtils.GetConvertedFilesExt(CurrentFileType);

        public FileType[] FileTypes { get; } = PackUtils.PacksInfo.Select(it => it.fileType).ToArray();

        [NotNull]
        private readonly IFileConverter _fileConverter = new FileConverter();
        private FileType _currentFileType = FileType.Texture;
        
        [NotNull]
        public IActionCommand<string[]> DropSourceFilesCommand { get; }
        [NotNull]
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
                        string resultsDir = Path.Combine(Path.GetDirectoryName(file), "converted_files");
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
                        string resultsDir = Path.Combine(Path.GetDirectoryName(file), "source_files");
                        IOUtils.EnsureDirExists(resultsDir);

                        string config = Path.ChangeExtension(file, PackUtils.PackFileConfigExtension);
                        if (!File.Exists(config))
                            config = null;
        
                        var (convertedFile, fileInfo) = await _fileConverter.ConvertToSource(CurrentFileType, file, config);
                        string resultFileExt = PackUtils.GetInitialFilesExt(CurrentFileType);

                        string resultFileName = Path.GetFileNameWithoutExtension(file);
                        if (fileInfo != null)
                        {
                            switch (CurrentFileType)
                            {
                                case FileType.Texture:
                                    var textureInfo = (TextureFileInfo) fileInfo;
                                    resultFileName = textureInfo.EntryName;
                                    break;
                                case FileType.Map:
                                    var mapInfo = (MapFileInfo) fileInfo;
                                    resultFileName = mapInfo.ResultFileName;
                                    break;
                                case FileType.Character:
                                    break;
                                case FileType.Gui:
                                    var guiInfo = (GuiFileInfo) fileInfo;
                                    resultFileName = guiInfo.EntryName;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
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
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
}