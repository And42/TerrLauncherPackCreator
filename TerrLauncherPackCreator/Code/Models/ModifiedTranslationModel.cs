using System.Collections.Generic;
using CrossPlatform.Code.Utils;

namespace TerrLauncherPackCreator.Code.Models;

public class ModifiedTranslationModel : ModifiedFileModel
{
    public string CurrentLanguage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool IgnoreForCategory
    {
        get;
        set => SetProperty(ref field, value);
    }

    public IReadOnlyList<string> ShortLanguages { get; }
        
    public ModifiedTranslationModel(string filePath, bool isDragDropTarget) : base(filePath, isDragDropTarget)
    {
        ShortLanguages = PackUtils.TranslationLanguages;
        CurrentLanguage = ShortLanguages[0];
    }
}