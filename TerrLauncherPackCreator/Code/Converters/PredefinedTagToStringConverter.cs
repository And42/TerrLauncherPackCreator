using System;
using System.Globalization;
using System.Windows.Data;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class PredefinedTagToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as PredefinedPackTag?) switch
            {
                PredefinedPackTag.Animated => StringResources.PredefinedPackTagAnimated,
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;

            if (val == StringResources.PredefinedPackTagAnimated)
                return PredefinedPackTag.Animated;
            return null;
        }
    }
}