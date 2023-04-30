using System.Windows;
using CommonLibrary.Resources.Localizations;

namespace CommonLibrary.CommonUtils;

public static class MessageBoxUtils
{
    public static void ShowError(string text)
    {
        MessageBox.Show(
            text,
            StringResources.ErrorLower,
            MessageBoxButton.OK,
            MessageBoxImage.Exclamation
        );
    }

    public static void ShowInformation(string text)
    {
        MessageBox.Show(
            text,
            StringResources.InformationLower,
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }
}