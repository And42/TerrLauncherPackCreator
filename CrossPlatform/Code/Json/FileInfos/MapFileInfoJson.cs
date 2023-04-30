using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class MapFileInfoJson
{
    [JsonPropertyName("result_file_name")]
    [JsonRequired]
    public string ResultFileName = null!;
}