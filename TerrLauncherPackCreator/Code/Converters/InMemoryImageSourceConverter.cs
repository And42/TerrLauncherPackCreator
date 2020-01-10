using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class InMemoryImageSourceConverter : ConverterBase<string, ImageSource>
    {
        public override ImageSource ConvertInternal(string value, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(value);
            image.EndInit();
            return image;
        }
    }
}