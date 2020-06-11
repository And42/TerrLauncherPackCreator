using System;
using System.Globalization;
using System.Windows.Data;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TextureFileInfo.TextureType casted))
                return null;
            
            return casted switch
            {
                TextureFileInfo.TextureType.General => StringResources.TextureTypeGeneral,
                TextureFileInfo.TextureType.Item => StringResources.TextureTypeItem,
                TextureFileInfo.TextureType.Npc => StringResources.TextureTypeNpc,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string casted))
                return null;
            
            if (casted == StringResources.TextureTypeGeneral)
                return TextureFileInfo.TextureType.General;
            if (casted == StringResources.TextureTypeItem)
                return TextureFileInfo.TextureType.Item;
            if (casted == StringResources.TextureTypeNpc)
                return TextureFileInfo.TextureType.Npc;
            return null;
        }
    }
}