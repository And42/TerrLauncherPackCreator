using System;
using System.Windows;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class DragDropUtils
    {
        public static void HandleDrag(
            [NotNull] DragEventArgs eventArgs,
            [NotNull] Predicate<string[]> areFilesValid,
            DragDropEffects validEffect
        )
        {
            eventArgs.Handled = true;

            if (!eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                eventArgs.Effects = DragDropEffects.None;
                return;
            }

            var files = (string[]) eventArgs.Data.GetData(DataFormats.FileDrop);
            if (files != null && areFilesValid(files))
                eventArgs.Effects = validEffect;
            else
                eventArgs.Effects = DragDropEffects.None;
        }

        public static void HandleDrop(
            [NotNull] DragEventArgs eventArgs,
            [NotNull] Predicate<string[]> areFilesValid,
            [NotNull] Action<string[]> handleValidFiles
        )
        {
            eventArgs.Handled = true;

            if (!eventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[]) eventArgs.Data.GetData(DataFormats.FileDrop);
            if (files != null && areFilesValid(files))
                handleValidFiles(files);
        }
    }
}