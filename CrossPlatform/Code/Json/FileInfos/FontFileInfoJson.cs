using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos;

internal class FontFileInfoJson
{
    [JsonProperty("entry_name")]
    public string? EntryName { get; set; }
}