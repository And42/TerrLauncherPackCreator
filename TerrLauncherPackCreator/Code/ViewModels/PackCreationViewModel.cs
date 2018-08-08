using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using MVVM_Tools.Code.Classes;
using MVVM_Tools.Code.Commands;
using MVVM_Tools.Code.Providers;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public class PackCreationViewModel : BindableBase
    {
        public Property<BitmapSource> Icon { get; }
        public Property<string> Title { get; }
        public Property<string> DescriptionRussian { get; }
        public Property<string> DescriptionEnglish { get; }
        public Property<Guid> Guid { get; }
        public Property<int> Version { get; }
        public Property<ObservableCollection<PreviewItemModel>> Previews { get; }

        public IActionCommand CreateNewGuidCommand { get; }
        public IActionCommand<string> DropIconCommand { get; }
        public IActionCommand<string[]> DropPreviewsCommand { get; }
        public IActionCommand<PreviewItemModel> DeletePreviewItemCommand { get; }

        public PackCreationViewModel()
        {
            Icon = new Property<BitmapSource>();
            Title = new Property<string>();
            DescriptionRussian = new Property<string>();
            DescriptionEnglish = new Property<string>();
            Guid = new Property<Guid>(System.Guid.NewGuid());
            Version = new Property<int>(1);
            Previews = new Property<ObservableCollection<PreviewItemModel>>(
                new ObservableCollection<PreviewItemModel>
                {
                    new PreviewItemModel(null, true)
                }
            );

            CreateNewGuidCommand = new ActionCommand(CreateNewGuidCommand_Execute);
            DropIconCommand = new ActionCommand<string>(DropIconCommand_Execute, DropIconCommand_CanExecute);
            DropPreviewsCommand = new ActionCommand<string[]>(DropPreviewsCommand_Execute, DropPreviewsCommand_CanExecute);
            DeletePreviewItemCommand = new ActionCommand<PreviewItemModel>(DeletePreviewItemCommand_Execute, DeletePreviewItemCommand_CanExecute);
        }

        private bool DeletePreviewItemCommand_CanExecute(PreviewItemModel previewItem)
        {
            return !previewItem.IsDragDropTarget;
        }

        private void DeletePreviewItemCommand_Execute(PreviewItemModel previewItem)
        {
            Previews.Value.Remove(previewItem);
        }

        private bool DropPreviewsCommand_CanExecute(string[] files)
        {
            return files != null && files.Any(it => File.Exists(it) && Path.GetExtension(it) == ".jpg");
        }

        private void DropPreviewsCommand_Execute(string[] files)
        {
            var filtered = files.Where(it => File.Exists(it) && Path.GetExtension(it) == ".jpg").ToArray();

            foreach (var file in filtered)
            {
                try
                {
                    var bitmap = new Bitmap(file);
                    Previews.Value.Insert(0, new PreviewItemModel(bitmap.ToBitmapSource(), false));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private bool DropIconCommand_CanExecute(string file)
        {
            return File.Exists(file) && Path.GetExtension(file) == ".png";
        }

        private void DropIconCommand_Execute(string file)
        {
            try
            {
                var bitmap = new Bitmap(file);
                Icon.Value = bitmap.ToBitmapSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(StringResources.LoadIconFromFileFailed, ex.Message),
                    StringResources.ErrorLower,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
            }
        }

        private void CreateNewGuidCommand_Execute()
        {
            Guid.Value = System.Guid.NewGuid();
        }
    }
}
