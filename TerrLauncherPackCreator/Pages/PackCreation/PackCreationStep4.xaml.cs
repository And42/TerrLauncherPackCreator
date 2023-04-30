using System;
using System.Diagnostics;
using System.Windows.Navigation;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Pages.PackCreation;

public partial class PackCreationStep4
{
    public PackCreationStep4(PackCreationViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
    }

    private PackCreationViewModel ViewModel
    {
        get => (DataContext as PackCreationViewModel).AssertNotNull();
        init => DataContext = value;
    }

    private void AuthorLink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
        catch (Exception ex)
        {
            MessageBoxUtils.ShowError(string.Format(StringResources.OpenLinkInBrowserFailed, ex.Message));
        }

        e.Handled = true;
    }
}