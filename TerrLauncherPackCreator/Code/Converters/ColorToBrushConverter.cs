using System.Globalization;
using System.Windows.Media;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

public class ColorToBrushConverter : ConverterBase<Color, SolidColorBrush>
{
    private SolidColorBrush? _latestBrush;
        
    public override SolidColorBrush ConvertInternal(Color value, object parameter, CultureInfo culture)
    {
        if (_latestBrush == null || _latestBrush.Color != value)
        {
            _latestBrush = new SolidColorBrush(value);
            _latestBrush.Freeze();
        }
        return _latestBrush;
    }

    public override Color ConvertBackInternal(SolidColorBrush value, object parameter, CultureInfo culture)
    {
        return value.Color;
    }

    protected override Color GetSourceIfNull()
    {
        return Colors.Transparent;
    }
}