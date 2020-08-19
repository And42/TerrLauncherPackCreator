using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TextureType = TerrLauncherPackCreator.Code.Json.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureModelToAnimateInGuiVisibility : IMultiValueConverter
    {
        private static readonly object VisibleObject = Visibility.Visible;
        private static readonly object CollapsedObject = Visibility.Collapsed;
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Visibility));

            bool animated = (bool) values[0];
            TextureType currentTextureType = (TextureType) values[1];

            return animated && currentTextureType == TextureType.Item ? VisibleObject : CollapsedObject;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}