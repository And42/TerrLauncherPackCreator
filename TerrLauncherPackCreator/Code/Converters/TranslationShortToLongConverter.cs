using System.Globalization;
using CommonLibrary.CommonUtils;
using CrossPlatform.Code.Utils;
using MVVM_Tools.Code.Classes;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Converters;

public class TranslationShortToLongConverter : ConverterBase<string, string>
{
    public override string ConvertInternal(string value, object parameter, CultureInfo culture)
    {
        return PackUtils.TranslationLanguageTitles[PackUtils.TranslationLanguages.IndexOf(value)];
    }

    public override string ConvertBackInternal(string value, object parameter, CultureInfo culture)
    {
        return PackUtils.TranslationLanguages[PackUtils.TranslationLanguageTitles.IndexOf(value)];
    }
}