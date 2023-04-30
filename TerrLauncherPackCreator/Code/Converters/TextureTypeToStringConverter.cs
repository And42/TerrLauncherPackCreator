using System;
using System.Globalization;
using System.Windows.Data;
using TerrLauncherPackCreator.Resources.Localizations;
using TextureType = CrossPlatform.Code.FileInfos.TextureFileInfo.TextureType;

namespace TerrLauncherPackCreator.Code.Converters;

public class TextureTypeToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TextureType casted)
            return null;
            
        return casted switch
        {
            TextureType.General => StringResources.TextureTypeGeneral,
            TextureType.ItemDeprecated => StringResources.TextureTypeItemDeprecated,
            TextureType.NpcDeprecated => StringResources.TextureTypeNpcDeprecated,
            TextureType.BuffDeprecated => StringResources.TextureTypeBuffDeprecated,
            TextureType.ExtraDeprecated => StringResources.TextureTypeExtraDeprecated,
            TextureType.Item => StringResources.TextureTypeItem,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string cast)
            return null;
            
        if (cast == StringResources.TextureTypeGeneral)
            return TextureType.General;
        if (cast == StringResources.TextureTypeItemDeprecated)
            return TextureType.ItemDeprecated;
        if (cast == StringResources.TextureTypeNpcDeprecated)
            return TextureType.NpcDeprecated;
        if (cast == StringResources.TextureTypeBuffDeprecated)
            return TextureType.BuffDeprecated;
        if (cast == StringResources.TextureTypeExtraDeprecated)
            return TextureType.ExtraDeprecated;
        if (cast == StringResources.TextureTypeItem)
            return TextureType.Item;
        return null;
    }
}