using System.Collections.ObjectModel;
using JetBrains.Annotations;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreator.Code.Enums;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedFilesGroupModel : BindableBase
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