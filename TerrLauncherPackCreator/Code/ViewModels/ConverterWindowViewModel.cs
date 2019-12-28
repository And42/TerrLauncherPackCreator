using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using MVVM_Tools.Code.Commands;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Interfaces;
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

        public string CurrentFilesExtension => PackUtils.PacksInfo.First(it => it.fileType == CurrentFileType).initialFilesExt;

        public FileType[] FileTypes { get; } = PackUtils.PacksInfo.Select(it => it.fileType).ToArray();

        [NotNull]
        private readonly IFileConverter _fileConverter = new FileConverter(Paths.TextureDefinitionsFile, null);
        private FileType _currentFileType = FileType.Texture;
        
        [NotNull]
        public IActionCommand<string[]> DropFilesCommand { get; }

        public ConverterWindowViewModel()
        {
            DropFilesCommand = new ActionCommand<string[]>(DropFilesCommand_Execute, DropFilesCommand_CanExecute);
            
            PropertyChanged += OnPropertyChanged;
        }

        private bool DropFilesCommand_CanExecute(string[] files)
        {
            if (Working)
                return false;

            string currentExt = CurrentFilesExtension;
            return files.All(it => Path.GetExtension(it) == currentExt);
        }

        private async void DropFilesCommand_Execute(string[] files)
        {
            using (LaunchWork())
            {
                try
                {
                    foreach (string file in files)
                    {
                        string resultsDir = Path.Combine(Path.GetDirectoryName(file), "converted_files");
                        Directory.CreateDirectory(resultsDir);
                        await _fileConverter.ConvertToTarget(CurrentFileType, file, resultsDir);
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
                    OnPropertyChanged(nameof(CurrentFilesExtension));
                    break;
                case nameof(Working):
                    DropFilesCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
    }
}