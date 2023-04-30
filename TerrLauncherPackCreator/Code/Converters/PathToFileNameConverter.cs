using System.Globalization;
using System.IO;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

public class PathToFileNameConverter : ConverterBase<string, string>
{
    public override string ConvertInternal(string value, object parameter, CultureInfo culture)
    {
        return Path.GetFileNameWithoutExtension(value);
    }
}