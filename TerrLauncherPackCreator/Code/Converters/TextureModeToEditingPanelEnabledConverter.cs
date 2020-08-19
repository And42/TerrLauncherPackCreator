using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using TextureType = TerrLauncherPackCreator.Code.Json.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureModeToEditingPanelEnabledConverter : IValueConverter
    {
        private static readonly object FalseObject = false;
        private static readonly object TrueObject = true;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(bool));

            var textureType = value as TextureType?;
            return textureType switch
            {
                TextureType.General => true,
                TextureType.ItemDeprecated => false,
                TextureType.NpcDeprecated => false,
                TextureType.BuffDeprecated => false,
                TextureType.ExtraDeprecated => false,
                TextureType.Item => true,
                null => false,
                _ => throw new ArgumentOutOfRangeException("Unknown texture type: " + textureType)
            } ? TrueObject : FalseObject;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}