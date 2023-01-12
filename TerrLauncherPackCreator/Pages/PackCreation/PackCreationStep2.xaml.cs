using System.Windows;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Models;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Pages.PackCreation
{
    public partial class PackCreationStep2
    {
        public PackCreationStep2(PackCreationViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        private PackCreationViewModel ViewModel
        {
            get => (DataContext as PackCreationViewModel).AssertNotNull();
            init => DataContext = value;
        }

        private void Previews_OnDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var files = (string[]?) e.Data.GetData(DataFormats.FileDrop);

            if (files != null && ViewModel.DropPreviewsCommand.CanExecute(files))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void Previews_OnDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[]?) e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
                ViewModel.DropPreviewsCommand.Execute(files);
        }

        private void PreviewImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var model = ((FrameworkElement) sender).DataContext as PreviewItemModel;
            if (model == null)
                return;

            model.ImageWidthDp = e.NewSize.Width;
            model.ImageHeightDp = e.NewSize.Height;
        }
    }
}
