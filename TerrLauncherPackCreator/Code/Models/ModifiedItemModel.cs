using System.Diagnostics;

namespace TerrLauncherPackCreator.Code.Models
{
    public class ModifiedItemModel
    {
        public string FilePath { get; }
        public bool IsDragDropTarget { get; }

        public ModifiedItemModel(string filePath, bool isDragDropTarget)
        {
            FilePath = filePath;
            IsDragDropTarget = isDragDropTarget;

            Debug.Assert(filePath != null ^ isDragDropTarget, "filePath != null ^ isDragDropTarget");
        }
    }
}
