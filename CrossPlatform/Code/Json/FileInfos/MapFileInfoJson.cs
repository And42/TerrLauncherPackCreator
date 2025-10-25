using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class MapFileInfoJson
{
    [JsonRequired]
    [JsonPropertyName("result_file_name")]
    public string ResultFileName = null!;
}