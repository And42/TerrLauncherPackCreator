using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using CrossPlatform.Code.Interfaces;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

public class StatusBarVisibilityConverter : ConverterBase<IEnumerable<IProgressManager>, Visibility>
{
    public override Visibility ConvertInternal(IEnumerable<IProgressManager> value, object parameter, CultureInfo culture)
    {
        return value.Any(it => it.RemainingFilesCount > 0) ? Visibility.Visible : Visibility.Collapsed;
    }
}