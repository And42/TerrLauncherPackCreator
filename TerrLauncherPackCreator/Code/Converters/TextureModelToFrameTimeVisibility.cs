using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static TerrLauncherPackCreator.Code.Json.TextureFileInfo;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureModelToFrameTimeVisibility : IMultiValueConverter
    {
        private static readonly IList<TextureType> FrameVisibleTypes = new List<TextureType> {
            TextureType.Item,
            TextureType.Npc,
            TextureType.Buff,
            TextureType.Extra
        };
        private static readonly object VisibleObject = Visibility.Visible;
        private static readonly object CollapsedObject = Visibility.Collapsed;
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(Visibility));

            bool animated = (bool) values[0];
            TextureType currentTextureType = (TextureType) values[1];

            return animated && FrameVisibleTypes.Contains(currentTextureType) ? VisibleObject : CollapsedObject;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}