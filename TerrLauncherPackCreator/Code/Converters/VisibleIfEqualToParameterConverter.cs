using System.Globalization;
using System.Windows;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreator.Code.Json;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class VisibleIfEqualToParameterConverter : ConverterBase<object, Visibility>
    {
        public override Visibility ConvertInternal(object value, object parameter, CultureInfo culture)
        {
            return Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}