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
    }
}
