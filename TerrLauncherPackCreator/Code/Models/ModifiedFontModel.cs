using System.Collections.ObjectModel;
using System.IO;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedFontModel : ModifiedFileModel
{
    public string? Prefix
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<string> CommonPrefixes { get; }

    public ModifiedFontModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CommonPrefixes =
        [
            "",
            "Content/Fonts"
        ];
        Prefix = CommonPrefixes[1];
    }
}