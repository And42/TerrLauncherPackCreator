using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class AudioFileInfoJson
{
    [JsonPropertyName("entry_name")]
    [JsonRequired]
    public string EntryName = null!;
}