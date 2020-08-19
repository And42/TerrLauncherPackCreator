using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;

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

            {
                const int fileTypesHandled = 7;
                const int _ = 1 / (fileTypesHandled / PackUtils.TotalFileTypes) +
                              1 / (PackUtils.TotalFileTypes / fileTypesHandled);
            }
            
            string resourceName;
            if (previewItem.IsDragDropTarget)
                resourceName = "ModifiedFileDropTargetDataTemplate";
            else switch (item)
            {
                case ModifiedTextureModel _:
                    resourceName = "ModifiedTextureTemplate";
                    break;
                case ModifiedGuiModel _:
                    resourceName = "ModifiedGuiTemplate";
                    break;
                case ModifiedFontModel _:
                    resourceName = "ModifiedFontTemplate";
                    break;
                case ModifiedMapModel _:
                    resourceName = "ModifiedMapTemplate";
                    break;
                case ModifiedCharacterModel _:
                    resourceName = "ModifiedCharacterTemplate";
                    break;
                case ModifiedTranslationModel _:
                    resourceName = "ModifiedTranslationTemplate";
                    break;
                case ModifiedAudioModel _:
                    resourceName = "ModifiedAudioTemplate";
                    break;
                default:
                    resourceName = "ModifiedFileDataTemplate";
                    break;
            }

            return (DataTemplate) containerUi.FindResource(resourceName);
        }
    }
}
