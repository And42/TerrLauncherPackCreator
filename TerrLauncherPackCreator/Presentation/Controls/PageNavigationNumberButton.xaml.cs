using System.Windows;

namespace TerrLauncherPackCreator.Presentation.Controls;

public partial class PageNavigationNumberButton
{
    public static readonly DependencyProperty PageNumberProperty = DependencyProperty.Register(
        nameof(PageNumber), typeof(int), typeof(PageNavigationNumberButton), new PropertyMetadata(0));

    public int PageNumber
    {
        get => (int) GetValue(PageNumberProperty);
        set => SetValue(PageNumberProperty, value);
    }

    public static readonly DependencyProperty InitialRadiusProperty = DependencyProperty.Register(
        nameof(InitialRadius), typeof(double), typeof(PageNavigationNumberButton), new PropertyMetadata(30.0));

    public double InitialRadius
    {
        get => (double) GetValue(InitialRadiusProperty);
        set => SetValue(InitialRadiusProperty, value);
    }

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(PageNavigationNumberButton), new PropertyMetadata(false, IsActiveChanged));

    public bool IsActive
    {
        get => (bool) GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public PageNavigationNumberButton()
    {
        InitializeComponent();
    }

    private static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pageButton = (PageNavigationNumberButton) d;
        var newState = (bool) e.NewValue;

        VisualStateManager.GoToElementState(pageButton.grid, newState ? "Active" : "Normal", true);
    }
}