using System.Collections.ObjectModel;
using System.IO;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedFontModel : ModifiedFileModel
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

        public ModifiedFontModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            CommonPrefixes = new ObservableCollection<string>
            {
                "",
                "Content/Fonts"
            };
            _prefix = CommonPrefixes[1];
        }
    }
}