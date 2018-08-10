using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class MainWindow
    {
        private static readonly Duration StepChangeAnimationPartDuration = new Duration(TimeSpan.FromSeconds(0.25 / 2));

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
            var fadeOutAnimation = new DoubleAnimation(0, StepChangeAnimationPartDuration);
            var fadeInAnimation = new DoubleAnimation(1, StepChangeAnimationPartDuration);

            fadeOutAnimation.Completed += (sender, args) =>
            {
                StepsFrame.Navigate(ViewModel.StepsPages.Value[ViewModel.CurrentStep.Value - 1]);
                StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeInAnimation);
            };

            StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
        }
    }
}
