using System;
using MVVM_Tools.Code.Commands;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedModModel : ModifiedFileModel
{
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    private string _id;

    public bool IgnoreForCategory
    {
        get => _ignoreForCategory;
        set => SetProperty(ref _ignoreForCategory, value);
    }
    private bool _ignoreForCategory;

    public IActionCommand GenerateNewGuidCommand { get; }
    
    public ModifiedModModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        _id = string.Empty;
        GenerateNewGuidCommand = new ActionCommand(GenerateNewGuid_Execute);
    }

    private void GenerateNewGuid_Execute()
    {
        Id = Guid.NewGuid().ToString();
    }
}