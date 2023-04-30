using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TerrLauncherPackCreator.Code.Models;

namespace TerrLauncherPackCreator.Code.TemplateSelectors;

public class PreviewsTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        Debug.Assert(item != null, "item != null");
        Debug.Assert(container != null, "container != null");

        var previewItem = (PreviewItemModel) item;
        var containerUi = (FrameworkElement) container;

        var imageTemplate = (DataTemplate) containerUi.FindResource("PreviewImageDataTemplate");
        var dropTargetTemplate = (DataTemplate) containerUi.FindResource("PreviewDropTargetDataTemplate");

        return previewItem.IsDragDropTarget ? dropTargetTemplate : imageTemplate;
    }
}