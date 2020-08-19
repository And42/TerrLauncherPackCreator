using System;
using System.Globalization;
using System.Windows.Data;
using TerrLauncherPackCreator.Resources.Localizations;
using TextureType = TerrLauncherPackCreator.Code.Json.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TextureType casted))
                return null;
            
            return casted switch
            {
                TextureType.General => StringResources.TextureTypeGeneral,
                TextureType.Item => StringResources.TextureTypeItem,
                TextureType.Npc => StringResources.TextureTypeNpc,
                TextureType.Buff => StringResources.TextureTypeBuff,
                TextureType.Extra => StringResources.TextureTypeExtra,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string cast))
                return null;
            
            if (cast == StringResources.TextureTypeGeneral)
                return TextureType.General;
            if (cast == StringResources.TextureTypeItem)
                return TextureType.Item;
            if (cast == StringResources.TextureTypeNpc)
                return TextureType.Npc;
            if (cast == StringResources.TextureTypeBuff)
                return TextureType.Buff;
            if (cast == StringResources.TextureTypeExtra)
                return TextureType.Extra;
            return null;
        }
    }
}