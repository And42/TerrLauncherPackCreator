using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Pages.PackCreation
{
    public partial class PackCreationStep1
    {
        private static readonly Duration PackOptionsMarginDuration = new Duration(TimeSpan.FromSeconds(0.2));

        public PackCreationStep1(PackCreationViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
        }

        public PackCreationViewModel ViewModel
        {
            get => DataContext as PackCreationViewModel;
            set => DataContext = value;
        }

        private void PackCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (packCreationOptions.Visibility == Visibility.Visible)
            {
                var margin = new ThicknessAnimation(new Thickness(0), PackOptionsMarginDuration);
                margin.Completed += (o, args) => packCreationOptions.Visibility = Visibility.Collapsed;

                packCreationOptions.BeginAnimation(StackPanel.MarginProperty, margin);
            }
            else
            {
                var margin = new ThicknessAnimation(new Thickness(5, packOptionsToggle.ActualHeight + 3, 5, 0), PackOptionsMarginDuration);

                packCreationOptions.BeginAnimation(StackPanel.MarginProperty, margin);

                packCreationOptions.Visibility = Visibility.Visible;
            }
        }
    }
}
