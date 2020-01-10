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
            else if (item is ModifiedTextureModel)
                resourceName = "ModifiedTextureTemplate";
            else
                resourceName = "ModifiedFileDataTemplate";

            return (DataTemplate) containerUi.FindResource(resourceName);
        }
    }
}
