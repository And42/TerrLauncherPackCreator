using System.Collections.ObjectModel;
using System.IO;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedGuiModel : ModifiedFileModel
    {
        [CanBeNull]
        public string Prefix
        {
            get => _prefix;
            set => SetProperty(ref _prefix, value);
        }
        private string _prefix;
        
        [CanBeNull]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _name;

        [NotNull]
        public ObservableCollection<string> CommonPrefixes { get; }
        
        public ModifiedGuiModel([NotNull] string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            CommonPrefixes = new ObservableCollection<string>
            {
                "",
                "Content/Images"
            };
            Prefix = CommonPrefixes[1];
        }
    }
}