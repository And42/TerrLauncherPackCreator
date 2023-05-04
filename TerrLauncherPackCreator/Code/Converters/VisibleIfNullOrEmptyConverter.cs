using System.Globalization;
using System.Windows;
using CommonLibrary.CommonUtils;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

internal class VisibleIfNullOrEmptyConverter : ConverterBase<string?, Visibility>
{
    public override Visibility ConvertInternal(string? value, object parameter, CultureInfo culture)
    {
        return value.IsNullOrEmpty() ? Visibility.Visible : Visibility.Collapsed;
    }
}