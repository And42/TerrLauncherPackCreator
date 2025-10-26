using System;
using System.Globalization;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreatorUpdater.Resources.Localizations;

namespace TerrLauncherPackCreatorUpdater.Code.Converters;

public class SpeedToStringConverter : ConverterBase<long, string>
{
    private static readonly string[] Ordinals = StringResources.SpeedOrdinals.Split(';');
    private static readonly string SpeedEnding = StringResources.SpeedEnding;

    public override string ConvertInternal(long speed, object parameter, CultureInfo culture)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(speed);

        var ordinal = 0;

        while (speed > 1024)
        {
            speed /= 1024;
            ordinal++;
        }

        return $"{Math.Round((decimal)speed, 1, MidpointRounding.AwayFromZero)} {Ordinals[ordinal]}{SpeedEnding}";
    }
}