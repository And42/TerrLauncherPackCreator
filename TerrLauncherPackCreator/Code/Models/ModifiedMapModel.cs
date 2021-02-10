using System.IO;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedMapModel : ModifiedFileModel
    {
        public string? ResultFileName
        {
            get => _resultFileName;
            set => SetProperty(ref _resultFileName, value);
        }
        private string? _resultFileName;

        public ModifiedMapModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _resultFileName = Path.GetFileNameWithoutExtension(filePath);
        }
    }
}