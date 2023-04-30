using System.Collections.ObjectModel;
using CrossPlatform.Code.Enums;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedFilesGroupModel : ViewModelBase
{
    public string Title { get; }
        
    public string FilesExtension { get; }
        
    public FileType FilesType { get; }
        
    public ObservableCollection<ModifiedFileModel> ModifiedFiles { get; } = new ObservableCollection<ModifiedFileModel>();

    public ModifiedFilesGroupModel(string title, string filesExtension, FileType filesType)
    {
        Title = title;
        FilesExtension = filesExtension;
        FilesType = filesType;
    }
}