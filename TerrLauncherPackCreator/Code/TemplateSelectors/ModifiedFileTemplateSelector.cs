using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
                case ModifiedMapModel _:
                    resourceName = "ModifiedMapTemplate";
                    break;
                default:
                    resourceName = "ModifiedFileDataTemplate";
                    break;
            }

            return (DataTemplate) containerUi.FindResource(resourceName);
        }
    }
}
