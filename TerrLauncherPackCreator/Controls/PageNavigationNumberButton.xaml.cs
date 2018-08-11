using System.Windows;

namespace TerrLauncherPackCreator.Controls
{
    public partial class PageNavigationNumberButton
    {
        public static readonly DependencyProperty PageNumberProperty = DependencyProperty.Register(
            "PageNumber", typeof(int), typeof(PageNavigationNumberButton), new PropertyMetadata(default(int)));

        public int PageNumber
        {
            get => (int) GetValue(PageNumberProperty);
            set => SetValue(PageNumberProperty, value);
        }

        public static readonly DependencyProperty InitialRadiusProperty = DependencyProperty.Register(
            "InitialRadius", typeof(double), typeof(PageNavigationNumberButton), new PropertyMetadata(30.0));

        public double InitialRadius
        {
            get => (double) GetValue(InitialRadiusProperty);
            set => SetValue(InitialRadiusProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(PageNavigationNumberButton), new PropertyMetadata(default(bool), IsActiveChanged));

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
}
