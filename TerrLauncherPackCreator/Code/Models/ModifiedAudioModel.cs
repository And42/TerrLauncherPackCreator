using System.Collections.ObjectModel;
using System.IO;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedAudioModel : ModifiedFileModel
{
    public string Prefix
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<string> CommonPrefixes { get; }
        
    public ModifiedAudioModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CommonPrefixes =
        [
            "",
            "Content/Sounds",
            "Content/Music"
        ];
        Prefix = CommonPrefixes[1];
    }
}