using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class MainWindow
    {
        private static readonly Duration StepChangeAnimationPartDuration = new Duration(TimeSpan.FromSeconds(0.25 / 2));

        private readonly Button[] _pageNavigationNumberButtons;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            _pageNavigationNumberButtons = PageNavigationNumberButtonsPanel.Children.Cast<Button>().ToArray();

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
            int currentPageIndex = ViewModel.CurrentStep.Value - 1;

            var fadeOutAnimation = new DoubleAnimation(0, StepChangeAnimationPartDuration);
            var fadeInAnimation = new DoubleAnimation(1, StepChangeAnimationPartDuration);

            fadeOutAnimation.Completed += (sender, args) =>
            {
                for (int i = 0; i < _pageNavigationNumberButtons.Length; i++)
                    _pageNavigationNumberButtons[i].IsEnabled = i == currentPageIndex;

                StepsFrame.Navigate(ViewModel.StepsPages.Value[currentPageIndex]);
                StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeInAnimation);
            };

            StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
        }
    }
}
