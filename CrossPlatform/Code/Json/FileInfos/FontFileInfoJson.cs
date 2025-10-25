using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class FontFileInfoJson
{
    [JsonPropertyName("entry_name")]
    public string? EntryName { get; set; }
}