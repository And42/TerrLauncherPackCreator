using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class AudioFileInfoJson
{
    [JsonRequired]
    [JsonPropertyName("entry_name")]
    public string EntryName = null!;
}