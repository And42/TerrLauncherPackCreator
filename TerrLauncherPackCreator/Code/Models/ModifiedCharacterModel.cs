using System.IO;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedCharacterModel : ModifiedFileModel
    {
        public string ResultFileName
        {
            get => _resultFileName;
            set => SetProperty(ref _resultFileName, value);
        }
        private string _resultFileName;

        public ModifiedCharacterModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _resultFileName = Path.GetFileNameWithoutExtension(filePath);
        }
    }
}