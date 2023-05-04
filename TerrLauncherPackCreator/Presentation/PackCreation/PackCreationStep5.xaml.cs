using CommonLibrary.CommonUtils;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

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