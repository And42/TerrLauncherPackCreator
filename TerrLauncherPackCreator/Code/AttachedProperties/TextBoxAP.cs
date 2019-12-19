using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace TerrLauncherPackCreator.Code.AttachedProperties
{
    public static class TextBoxAP
    {
        public static readonly DependencyProperty AutoScrollToEndProperty = DependencyProperty.RegisterAttached(
            "AutoScrollToEnd", typeof(bool), typeof(TextBoxAP), new PropertyMetadata(default(bool), AutoScrollToEndChanged));

        private static void AutoScrollToEndChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
                ((TextBox) element).TextChanged += OnTextChanged;
            else
                ((TextBox) element).TextChanged -= OnTextChanged;
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox) sender).ScrollToEnd();
        }

        public static bool GetAutoScrollToEnd([NotNull] DependencyObject element)
        {
            return (bool) element.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd([NotNull] DependencyObject element, bool value)
        {
            element.SetValue(AutoScrollToEndProperty, value);
        }
    }
}