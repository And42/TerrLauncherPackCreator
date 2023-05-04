using System;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Implementations;

namespace TerrLauncherPackCreator.Presentation;

public partial class PackStartupWindow
{
    private PackStartupWindowViewModel ViewModel
    {
        get => (DataContext as PackStartupWindowViewModel).AssertNotNull();
        init => DataContext = value;
    }

    public PackStartupWindow()
    {
        InitializeComponent();

        ViewModel = new PackStartupWindowViewModel(
            new AttachedWindowManipulator(this),
            ValuesProvider.AppSettings
        )
        {
            RecreateWindow = RecreateWindow
        };
    }

    private void PackStartupWindow_OnSourceInitialized(object sender, EventArgs e)
    {
        WindowUtils.RemoveIcon(this);
    }
        
    private void RecreateWindow()
    {
        new PackStartupWindow().Show();
        Close();
    }
}