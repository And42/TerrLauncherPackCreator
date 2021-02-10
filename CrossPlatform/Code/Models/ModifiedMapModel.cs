using System.IO;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedMapModel : ModifiedFileModel
    {
        [CanBeNull]
        public string ResultFileName
        {
            get => _resultFileName;
            set => SetProperty(ref _resultFileName, value);
        }
        [CanBeNull]
        private string _resultFileName;

        public ModifiedMapModel([NotNull] string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
        {
            _resultFileName = Path.GetFileNameWithoutExtension(filePath);
        }
    }
}