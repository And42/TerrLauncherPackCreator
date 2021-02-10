using System.IO;
using JetBrains.Annotations;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedFileModel : ViewModelBase
    {
        [NotNull]
        public string FilePath { get; }
        [NotNull]
        public string FileExtension { get; }
        public bool IsDragDropTarget { get; }

        public ModifiedFileModel([NotNull] string filePath, bool isDragDropTarget)
        {
            FilePath = filePath;
            FileExtension = Path.GetExtension(filePath);
            IsDragDropTarget = isDragDropTarget;
        }

        public override string ToString()
        {
            return $"FilePath: \"{FilePath}\"; IsDragDropTarget: \"{IsDragDropTarget.ToString()}\"";
        }
    }
}
