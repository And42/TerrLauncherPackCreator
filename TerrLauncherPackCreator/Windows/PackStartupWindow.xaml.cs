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
        public PackStartupWindowViewModel ViewModel
        {
            get => DataContext as PackStartupWindowViewModel;
            set => DataContext = value;
        }

        public PackStartupWindow()
        {
            InitializeComponent();

            ViewModel = new PackStartupWindowViewModel(new AttachedWindowManipulator(this));
        }

        private void PackStartupWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            WindowUtils.RemoveIcon(this);
        }
    }
}
