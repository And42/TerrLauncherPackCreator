using System.Globalization;
using System.Windows;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

internal class VisibleIfNullConverter : ConverterBase<object?, Visibility>
{
    public override Visibility ConvertInternal(object? value, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }
}