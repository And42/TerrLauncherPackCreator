using CrossPlatform.Code.Interfaces;
using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos;

internal class ModFileInfoJson : IPackFileInfo
{
    [JsonProperty("id", Required = Required.Always)]
    public string Id = null!;

    [JsonProperty("ignore_for_category")]
    public bool IgnoreForCategory;
}