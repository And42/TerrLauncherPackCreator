using System;
using System.Windows;
using System.Windows.Shell;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreatorUpdater.Code.ViewModels;

namespace TerrLauncherPackCreatorUpdater.Windows
{
    public partial class UpdaterWindow
    {
        public UpdaterWindowViewModel ViewModel
        {
            get => DataContext as UpdaterWindowViewModel;
            set => DataContext = value;
        }

        public UpdaterWindow()
        {
            InitializeComponent();
            
            ViewModel = new UpdaterWindowViewModel(TaskbarItemInfo = new TaskbarItemInfo());
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartLoading();
        }

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            WindowUtils.RemoveIcon(this);
        }
    }
}
