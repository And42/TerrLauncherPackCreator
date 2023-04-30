using System.Globalization;
using System.Windows;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

public class AnyToVisibleConverter : ConverterBase<int, Visibility>
{
    public override Visibility ConvertInternal(int value, object parameter, CultureInfo culture)
    {
        return value > 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}