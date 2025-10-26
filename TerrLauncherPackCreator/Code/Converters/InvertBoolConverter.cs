using System;
using System.Globalization;
using System.Windows.Data;

namespace TerrLauncherPackCreator.Code.Converters;

public class InvertBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as bool?) switch
        {
            false => true,
            true => false,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value as bool?) switch
        {
            false => true,
            true => false,
            _ => null
        };
    }
}