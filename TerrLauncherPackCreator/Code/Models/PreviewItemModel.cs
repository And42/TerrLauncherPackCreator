using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PreviewItemModel
    {
        public BitmapSource Image { get; }
        public bool IsDragDropTarget { get; }

        public PreviewItemModel(BitmapSource image, bool isDragDropTarget)
        {
            Image = image;
            IsDragDropTarget = isDragDropTarget;

            Debug.Assert(image != null ^ isDragDropTarget, "image != null ^ isDragDropTarget");
        }
    }
}
