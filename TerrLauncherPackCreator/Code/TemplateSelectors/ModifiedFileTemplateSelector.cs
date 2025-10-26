using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Utils;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.TemplateSelectors;

public class ModifiedFileTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object? item, DependencyObject container)
    {
        Debug.Assert(item != null);
        Debug.Assert(container != null);

        var previewItem = (ModifiedFileModel) item;
        var containerUi = (FrameworkElement) container;

        (1 / (8 / FileTypeEnum.Length)).Ignore();

        string resourceName;
        if (previewItem.IsDragDropTarget)
            resourceName = "ModifiedFileDropTargetDataTemplate";
        else
            resourceName = item switch
            {
                ModifiedTextureModel => "ModifiedTextureTemplate",
                ModifiedGuiModel => "ModifiedGuiTemplate",
                ModifiedFontModel => "ModifiedFontTemplate",
                ModifiedMapModel => "ModifiedMapTemplate",
                ModifiedCharacterModel => "ModifiedCharacterTemplate",
                ModifiedTranslationModel => "ModifiedTranslationTemplate",
                ModifiedAudioModel => "ModifiedAudioTemplate",
                ModifiedModModel => "ModifiedModTemplate",
                _ => "ModifiedFileDataTemplate"
            };

        return (DataTemplate) containerUi.FindResource(resourceName);
    }
}