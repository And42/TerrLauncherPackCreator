using System.Globalization;
using System.IO;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters {
    
    public class FileToMemoryStreamConverter : ConverterBase<string, MemoryStream> {

        public override MemoryStream? ConvertInternal(string? file, object? parameter, CultureInfo culture) {
            return File.Exists(file) ? new MemoryStream(File.ReadAllBytes(file)) : null;
        }
    }
}