using System;
using System.Globalization;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreatorUpdater.Code.Converters;

public class ProgressToArcEndConverter : ConverterBase<int, double>
{
    public override double ConvertInternal(int value, object parameter, CultureInfo culture)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value));

        return value * 360 / 100.0;
    }
}