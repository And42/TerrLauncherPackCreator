﻿using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace TerrLauncherPackCreator.Code.Models
{
    public class PreviewItemModel
    {
        public BitmapSource Image { get; }
        public string FilePath { get; }
        public bool IsDragDropTarget { get; }

        public PreviewItemModel(BitmapSource image, string filePath, bool isDragDropTarget)
        {
            Image = image;
            FilePath = filePath;
            IsDragDropTarget = isDragDropTarget;

            Debug.Assert(image != null ^ isDragDropTarget, "image != null ^ isDragDropTarget");
            Debug.Assert(filePath != null ^ isDragDropTarget, "filePath != null ^ isDragDropTarget");
        }
    }
}