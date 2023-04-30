using System.Globalization;
using System.IO;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters;

public class BytesToStream : ConverterBase<byte[], Stream>
{
    public override Stream? ConvertInternal(byte[]? value, object? parameter, CultureInfo culture)
    {
        return value == null ? null : new MemoryStream(value);
    }
}