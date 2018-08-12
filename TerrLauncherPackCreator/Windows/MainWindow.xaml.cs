﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TerrLauncherPackCreator.Code.ViewModels;
using TerrLauncherPackCreator.Controls;

namespace TerrLauncherPackCreator.Windows
{
    public partial class MainWindow
    {
        private static readonly Duration StepChangeAnimationPartDuration = new Duration(TimeSpan.FromSeconds(0.25 / 2));

        private PageNavigationNumberButton[] _pageNavigationNumberButtons;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            InitPagingButtons();
            UpdateStepPage();

            ViewModel.CurrentStep.PropertyChanged += (sender, args) => UpdateStepPage();
        }

        public MainWindowViewModel ViewModel
        {
            get => DataContext as MainWindowViewModel;
            set => DataContext = value;
        }

        private void InitPagingButtons()
        {
            PageNavigationNumberButtonsPanel.Children.Clear();

            int totalPages = ViewModel.StepsPages.Value.Length;
            _pageNavigationNumberButtons = new PageNavigationNumberButton[totalPages];

            for (int i = 1; i <= totalPages; i++)
            {
                var pageButton = new PageNavigationNumberButton
                {
                    PageNumber = i
                };

                if (i > 1)
                {
                    pageButton.Margin = new Thickness(10, 0, 0, 0);
                }

                pageButton.MouseLeftButtonDown += (sender, args) =>
                {
                    ViewModel.CurrentStep.Value = ((PageNavigationNumberButton) sender).PageNumber;
                };

                PageNavigationNumberButtonsPanel.Children.Add(pageButton);
                _pageNavigationNumberButtons[i - 1] = pageButton;
            }
        }

        private void UpdateStepPage()
        {
            int currentPageIndex = ViewModel.CurrentStep.Value - 1;

            var fadeOutAnimation = new DoubleAnimation(0, StepChangeAnimationPartDuration);
            var fadeInAnimation = new DoubleAnimation(1, StepChangeAnimationPartDuration);

            fadeOutAnimation.Completed += (sender, args) =>
            {
                for (int i = 0; i < _pageNavigationNumberButtons.Length; i++)
                    _pageNavigationNumberButtons[i].IsActive = i == currentPageIndex;

                StepsFrame.Navigate(ViewModel.StepsPages.Value[currentPageIndex]);
                StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeInAnimation);
            };

            StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
        }
    }
}