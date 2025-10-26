using System.Collections.ObjectModel;
using System.IO;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedGuiModel : ModifiedFileModel
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
        
    public ModifiedGuiModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        CommonPrefixes =
        [
            "",
            "Content/Images",
            "Content/Images/SplashScreens",
            "Content/Images/UI",
            "Content/Images/UI/Minimap/Default",
            "Content/Images/UI/PlayerResourceSets/FancyClassic",
            "Content/Images/UI/PlayerResourceSets/HorizontalBars",
            "Content/Images/UI/WorldGen"
        ];
        Prefix = CommonPrefixes[1];
    }
}