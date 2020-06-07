using System;
using System.Globalization;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreator.Code.Json;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Converters
{
    public class TextureTypeToStringConverter : ConverterBase<TextureFileInfo.TextureType, string>
    {
        public override string ConvertInternal(TextureFileInfo.TextureType value, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TextureFileInfo.TextureType.General => StringResources.TextureTypeGeneral,
                TextureFileInfo.TextureType.Item => StringResources.TextureTypeItem,
                TextureFileInfo.TextureType.Npc => StringResources.TextureTypeNpc,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public override TextureFileInfo.TextureType ConvertBackInternal(string value, object parameter, CultureInfo culture)
        {
            if (value == StringResources.TextureTypeGeneral)
                return TextureFileInfo.TextureType.General;
            if (value == StringResources.TextureTypeItem)
                return TextureFileInfo.TextureType.Item;
            if (value == StringResources.TextureTypeNpc)
                return TextureFileInfo.TextureType.Npc;
            throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
}