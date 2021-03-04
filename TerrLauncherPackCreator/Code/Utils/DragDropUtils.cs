using System;
using System.Windows;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class DragDropUtils
    {
        public static void HandleDrag(
            DragEventArgs eventArgs,
            Predicate<string[]> areFilesValid,
            DragDropEffects validEffect
        )
        {
            eventArgs.Handled = true;

            if (!eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                eventArgs.Effects = DragDropEffects.None;
                return;
            }

            var files = (string[]?) eventArgs.Data.GetData(DataFormats.FileDrop);
            if (files != null && areFilesValid(files))
                eventArgs.Effects = validEffect;
            else
                eventArgs.Effects = DragDropEffects.None;
        }

        public static void HandleDrop(
            DragEventArgs eventArgs,
            Predicate<string[]> areFilesValid,
            Action<string[]> handleValidFiles
        )
        {
            eventArgs.Handled = true;

            if (!eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[]?) eventArgs.Data.GetData(DataFormats.FileDrop);
            if (files != null && areFilesValid(files))
                handleValidFiles(files);
        }
    }
}