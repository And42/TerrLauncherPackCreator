using System.Collections.ObjectModel;
using TerrLauncherPackCreator.Code.Enums;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedFilesGroupModel : ViewModelBase
    {
        [NotNull]
        public string Title { get; }
        
        [NotNull]
        public string FilesExtension { get; }
        
        public FileType FilesType { get; }
        
        [NotNull]
        public ObservableCollection<ModifiedFileModel> ModifiedFiles { get; } = new ObservableCollection<ModifiedFileModel>();

        public ModifiedFilesGroupModel([NotNull] string title, [NotNull] string filesExtension, FileType filesType)
        {
            Title = title;
            FilesExtension = filesExtension;
            FilesType = filesType;
        }
    }
}