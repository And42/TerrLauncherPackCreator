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
                TextureFileInfo.TextureType.Buff => StringResources.TextureTypeBuff,
                TextureFileInfo.TextureType.Extra => StringResources.TextureTypeExtra,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string cast))
                return null;
            
            if (cast == StringResources.TextureTypeGeneral)
                return TextureFileInfo.TextureType.General;
            if (cast == StringResources.TextureTypeItem)
                return TextureFileInfo.TextureType.Item;
            if (cast == StringResources.TextureTypeNpc)
                return TextureFileInfo.TextureType.Npc;
            if (cast == StringResources.TextureTypeBuff)
                return TextureFileInfo.TextureType.Buff;
            if (cast == StringResources.TextureTypeExtra)
                return TextureFileInfo.TextureType.Extra;
            return null;
        }
    }
}