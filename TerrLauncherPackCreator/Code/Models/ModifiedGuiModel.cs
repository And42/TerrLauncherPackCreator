using System.Collections.ObjectModel;
using System.IO;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedGuiModel : ModifiedFileModel
    {
        public string? Prefix
        {
            get => _prefix;
            set => SetProperty(ref _prefix, value);
        }
        private string? _prefix;
        
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string? _name;

        public ObservableCollection<string> CommonPrefixes { get; }
        
        public ModifiedGuiModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            CommonPrefixes = new ObservableCollection<string>
            {
                "",
                "Content/Images",
                "Content/Images/SplashScreens",
                "Content/Images/UI",
                "Content/Images/UI/Minimap/Default",
                "Content/Images/UI/PlayerResourceSets/FancyClassic",
                "Content/Images/UI/PlayerResourceSets/HorizontalBars",
                "Content/Images/UI/WorldGen"
            };
            Prefix = CommonPrefixes[1];
        }
    }
}