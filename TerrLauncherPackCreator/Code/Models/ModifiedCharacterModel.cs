using System.IO;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedCharacterModel : ModifiedFileModel
{
    public string ResultFileName
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    public ModifiedCharacterModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        ResultFileName = Path.GetFileNameWithoutExtension(filePath);
    }
}