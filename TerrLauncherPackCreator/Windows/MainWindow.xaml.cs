using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            UpdateStepPage();

            ViewModel.CurrentStep.PropertyChanged += (sender, args) => UpdateStepPage();
        }

        public MainWindowViewModel ViewModel
        {
            get => DataContext as MainWindowViewModel;
            set => DataContext = value;
        }

        private void UpdateStepPage()
        {
            StepsFrame.Navigate(ViewModel.StepsPages.Value[ViewModel.CurrentStep.Value - 1]);
        }
    }
}
