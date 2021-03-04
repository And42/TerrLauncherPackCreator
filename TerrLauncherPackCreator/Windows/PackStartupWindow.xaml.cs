using System;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class PackStartupWindow
    {
        public PackStartupWindowViewModel ViewModel
        {
            get => DataContext as PackStartupWindowViewModel ?? throw new InvalidOperationException();
            set => DataContext = value;
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
}
