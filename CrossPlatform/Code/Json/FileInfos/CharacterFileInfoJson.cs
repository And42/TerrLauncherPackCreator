using System.Text.Json.Serialization;

namespace CrossPlatform.Code.Json.FileInfos;

internal class CharacterFileInfoJson
{
    [JsonPropertyName("result_file_name")]
    [JsonRequired]
    public string ResultFileName = null!;
}