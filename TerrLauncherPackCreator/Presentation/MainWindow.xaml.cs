﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CommonLibrary.CommonUtils;
using TerrLauncherPackCreator.Presentation.Controls;

namespace TerrLauncherPackCreator.Presentation;

public partial class MainWindow
{
    private static readonly Duration StepChangeAnimationPartDuration = new(TimeSpan.FromSeconds(0.25 / 2));

    private PageNavigationNumberButton[]? _pageNavigationNumberButtons;

    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainWindowViewModel(RestartApp);
        // setting window size does not work with xaml binding :(
        Width = ViewModel.InitialWindowWidth;
        Height = ViewModel.InitialWindowHeight;

        InitPagingButtons();
        UpdateStepPage();

        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
    }

    public MainWindowViewModel ViewModel
    {
        get => (DataContext as MainWindowViewModel).AssertNotNull();
        private init => DataContext = value;
    }

    private void InitPagingButtons()
    {
        PageNavigationNumberButtonsPanel.Children.Clear();

        int totalPages = ViewModel.StepsPages.Length;
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

            pageButton.MouseLeftButtonDown += (sender, _) =>
            {
                ViewModel.CurrentStep = ((PageNavigationNumberButton) sender).PageNumber;
            };

            PageNavigationNumberButtonsPanel.Children.Add(pageButton);
            _pageNavigationNumberButtons[i - 1] = pageButton;
        }
    }

    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    private void UpdateStepPage()
    {
        int currentPageIndex = ViewModel.CurrentStep - 1;

        var fadeOutAnimation = new DoubleAnimation(0, StepChangeAnimationPartDuration);
        var fadeInAnimation = new DoubleAnimation(1, StepChangeAnimationPartDuration);

        fadeOutAnimation.Completed += (_, _) =>
        {
            var buttons = _pageNavigationNumberButtons.AssertNotNull();
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].IsActive = i == currentPageIndex;

            StepsFrame.Navigate(ViewModel.StepsPages[currentPageIndex]);
            StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeInAnimation);
        };

        StepsFrame.BeginAnimation(Frame.OpacityProperty, fadeOutAnimation);
    }

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ViewModel.CurrentStep):
                UpdateStepPage();
                break;
        }
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        ViewModel.OnWindowClosed((int) ActualWidth, (int) ActualHeight);
    }

    private void RestartApp()
    {
        new PackStartupWindow().Show();
        Close();
    }
}