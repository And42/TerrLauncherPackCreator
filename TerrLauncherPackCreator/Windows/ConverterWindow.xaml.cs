using System.Windows;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class ConverterWindow
    {
        public ConverterWindow()
        {
            InitializeComponent();
            
            ViewModel = new ConverterWindowViewModel();
        }

        public ConverterWindowViewModel ViewModel
        {
            get => DataContext as ConverterWindowViewModel;
            set => DataContext = value;
        }

        private void Files_OnDragOver(object sender, DragEventArgs e)
        {
            DragDropUtils.HandleDrag(e, ViewModel.DropFilesCommand.CanExecute, DragDropEffects.Copy);
        }

        private void Files_OnDrop(object sender, DragEventArgs e)
        {
            DragDropUtils.HandleDrop(e, ViewModel.DropFilesCommand.CanExecute, ViewModel.DropFilesCommand.Execute);
        }
    }
}