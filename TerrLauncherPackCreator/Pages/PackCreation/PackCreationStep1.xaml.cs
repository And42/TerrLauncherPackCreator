using System.Windows;
using System.Windows.Input;
using CrossPlatform.Code.Enums;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Pages.PackCreation
{
    public partial class PackCreationStep1
    {
        public PackCreationStep1(PackCreationViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        public PackCreationViewModel ViewModel
        {
            get => DataContext as PackCreationViewModel;
            set => DataContext = value;
        }

        private void PredefinedTag_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedTag = (PredefinedPackTag) ((FrameworkElement) sender).DataContext;
            if (ViewModel.AddSelectedTagCommand.CanExecute(selectedTag))
                ViewModel.AddSelectedTagCommand.Execute(selectedTag);
        }
    }
}
