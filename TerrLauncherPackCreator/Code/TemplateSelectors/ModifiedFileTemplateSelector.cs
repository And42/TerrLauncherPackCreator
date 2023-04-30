using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CrossPlatform.Code.Enums;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.TemplateSelectors
{
    public class ModifiedFileTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Debug.Assert(item != null, "item != null");
            Debug.Assert(container != null, "container != null");

            var previewItem = (ModifiedFileModel) item;
            var containerUi = (FrameworkElement) container;

#pragma warning disable 219
            const int _ = 1 / (8 / (int) FileType.LastEnumElement);
#pragma warning restore 219

            string resourceName;
            if (previewItem.IsDragDropTarget)
                resourceName = "ModifiedFileDropTargetDataTemplate";
            else
                resourceName = item switch
                {
                    ModifiedTextureModel _ => "ModifiedTextureTemplate",
                    ModifiedGuiModel _ => "ModifiedGuiTemplate",
                    ModifiedFontModel _ => "ModifiedFontTemplate",
                    ModifiedMapModel _ => "ModifiedMapTemplate",
                    ModifiedCharacterModel _ => "ModifiedCharacterTemplate",
                    ModifiedTranslationModel _ => "ModifiedTranslationTemplate",
                    ModifiedAudioModel _ => "ModifiedAudioTemplate",
                    ModifiedModModel => "ModifiedModTemplate",
                    _ => "ModifiedFileDataTemplate"
                };

            return (DataTemplate) containerUi.FindResource(resourceName);
        }
    }
}
