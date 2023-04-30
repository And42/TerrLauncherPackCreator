using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Pages.PackCreation;

public partial class PackCreationStep5
{
    public PackCreationStep5(PackCreationViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
    }

    private PackCreationViewModel ViewModel
    {
        get => (DataContext as PackCreationViewModel).AssertNotNull();
        init => DataContext = value;
    }
}