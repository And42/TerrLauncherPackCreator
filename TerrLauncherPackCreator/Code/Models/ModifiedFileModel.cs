using System.IO;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedFileModel
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

        public static ModifiedFileModel FromFile(string filePath)
        {
            return new ModifiedFileModel(filePath, false);
        }

        public override string ToString()
        {
            return $"FilePath: \"{FilePath}\"; IsDragDropTarget: \"{IsDragDropTarget.ToString()}\"";
        }
    }
}
