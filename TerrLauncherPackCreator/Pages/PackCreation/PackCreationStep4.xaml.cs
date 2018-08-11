using System.Windows;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Pages.PackCreation
{
    public partial class PackCreationStep4
    {
        public PackCreationStep4(PackCreationViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        public PackCreationViewModel ViewModel
        {
            get => DataContext as PackCreationViewModel;
            set => DataContext = value;
        }

        private void ModifiedFiles_OnDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && ViewModel.DropModifiedFileCommand.CanExecute(files))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void ModifiedFiles_OnDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
                ViewModel.DropModifiedFileCommand.Execute(files);
        }
    }
}
