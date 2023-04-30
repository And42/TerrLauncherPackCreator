using System.Text.Json.Serialization;
using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.Json.FileInfos;

internal class TranslationFileInfoJson : IPackFileInfo
{
    [JsonPropertyName("language")]
    [JsonRequired]
    public string Language = null!;

    [JsonPropertyName("ignore_for_category")]
    public bool IgnoreForCategory;
}