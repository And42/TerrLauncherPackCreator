using System.IO;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedFileModel : ViewModelBase
{
    public string FilePath { get; }
    public string FileExtension { get; }
    public bool IsDragDropTarget { get; }

    public ModifiedFileModel(string filePath, bool isDragDropTarget)
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