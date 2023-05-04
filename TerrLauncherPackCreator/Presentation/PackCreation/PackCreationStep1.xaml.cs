using System.Windows;
using System.Windows.Input;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Enums;

namespace TerrLauncherPackCreator.Presentation.PackCreation;

public partial class PackCreationStep1
{
    public PackCreationStep1(PackCreationViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
    }

    private PackCreationViewModel ViewModel
    {
        get => (DataContext as PackCreationViewModel).AssertNotNull();
        init => DataContext = value;
    }

    private void PredefinedTag_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var selectedTag = (PredefinedPackTag) ((FrameworkElement) sender).DataContext;
        if (ViewModel.AddSelectedTagCommand.CanExecute(selectedTag))
            ViewModel.AddSelectedTagCommand.Execute(selectedTag);
    }
}