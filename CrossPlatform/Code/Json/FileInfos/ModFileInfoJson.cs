using System.Text.Json.Serialization;
using CrossPlatform.Code.Interfaces;

namespace CrossPlatform.Code.Json.FileInfos;

internal class ModFileInfoJson : IPackFileInfo
{
    [JsonPropertyName("id")]
    [JsonRequired]
    public string Id = null!;

    [JsonPropertyName("ignore_for_category")]
    public bool IgnoreForCategory;
}