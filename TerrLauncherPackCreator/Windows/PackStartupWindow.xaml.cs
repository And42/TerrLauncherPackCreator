using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class PackStartupWindow
    {
        public WindowsPackStartupWindowViewModel ViewModel
        {
            get => DataContext as WindowsPackStartupWindowViewModel;
            set => DataContext = value;
        }

        public PackStartupWindow()
        {
            InitializeComponent();

            ViewModel = new WindowsPackStartupWindowViewModel(
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
