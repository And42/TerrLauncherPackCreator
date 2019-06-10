using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PreviewItemModel
    {
        [CanBeNull]
        public Uri ImageUri { get; }
        [CanBeNull]
        public string FilePath { get; }
        public bool IsDragDropTarget { get; }

        public PreviewItemModel([CanBeNull] string filePath, bool isDragDropTarget)
        {
            ImageUri = filePath != null ? new Uri(filePath) : null;
            FilePath = filePath;
            IsDragDropTarget = isDragDropTarget;

            Debug.Assert(filePath != null ^ isDragDropTarget, "filePath != null ^ isDragDropTarget");
        }

        public static PreviewItemModel FromImageFile([NotNull] string filePath)
        {
            return new PreviewItemModel(filePath, false);
        }
    }
}
