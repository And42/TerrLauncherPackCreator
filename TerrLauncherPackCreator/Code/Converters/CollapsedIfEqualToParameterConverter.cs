using System.Globalization;
using System.Windows;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class CollapsedIfEqualToParameterConverter : ConverterBase<object, Visibility>
    {
        public override Visibility ConvertInternal(object? value, object? parameter, CultureInfo culture)
        {
            return Equals(value, parameter) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}