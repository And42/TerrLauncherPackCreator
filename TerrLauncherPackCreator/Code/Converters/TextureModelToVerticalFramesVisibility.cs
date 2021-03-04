using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static CrossPlatform.Code.FileInfos.TextureFileInfo;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureModelToVerticalFramesVisibility : IMultiValueConverter
    {
        private static readonly IList<TextureType> VerticalFramesVisibleTypes = new List<TextureType> {
            TextureType.ItemDeprecated,
            TextureType.BuffDeprecated,
            TextureType.ExtraDeprecated
        };
        private static readonly object VisibleObject = Visibility.Visible;
        private static readonly object CollapsedObject = Visibility.Collapsed;
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Visibility));

            bool animated = (bool) values[0];
            TextureType currentTextureType = (TextureType) values[1];

            return animated && VerticalFramesVisibleTypes.Contains(currentTextureType) ? VisibleObject : CollapsedObject;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}