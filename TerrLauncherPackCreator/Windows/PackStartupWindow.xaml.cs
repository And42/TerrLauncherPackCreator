﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Windows
{
    public partial class PackStartupWindow : Window
    {
        private static readonly Duration PackOptionsMarginDuration = new Duration(TimeSpan.FromSeconds(0.2));

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

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowUtils.RemoveIcon(this);
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