using System;
using MVVM_Tools.Code.Commands;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedModModel : ModifiedFileModel
{
    public string Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IgnoreForCategory
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IActionCommand GenerateNewGuidCommand { get; }
    
    public ModifiedModModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        Id = string.Empty;
        GenerateNewGuidCommand = new ActionCommand(GenerateNewGuid_Execute);
    }

    private void GenerateNewGuid_Execute()
    {
        Id = Guid.NewGuid().ToString();
    }
}