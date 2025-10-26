using System.IO;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedMapModel : ModifiedFileModel
{
    public string? ResultFileName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ModifiedMapModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        ResultFileName = Path.GetFileNameWithoutExtension(filePath);
    }
}