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

            var imageTemplate = (DataTemplate) containerUi.FindResource("ModifiedFileDataTemplate");
            var dropTargetTemplate = (DataTemplate) containerUi.FindResource("ModifiedFileDropTargetDataTemplate");

            return previewItem.IsDragDropTarget ? dropTargetTemplate : imageTemplate;
        }
    }
}
