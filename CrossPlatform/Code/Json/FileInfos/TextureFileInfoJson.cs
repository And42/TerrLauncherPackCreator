using CrossPlatform.Code.FileInfos;
using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.FileInfos;

internal class TextureFileInfoJson
{
    [JsonProperty("type", Required = Required.Always)]
    public TextureFileInfo.TextureType Type;

    [JsonProperty("entry_name")]
    public string? EntryName;

    [JsonProperty("element_id")]
    public int? ElementId;

    [JsonProperty("animated")]
    public bool? Animated;

    [JsonProperty("animate_in_gui")]
    public bool? AnimateInGui;

    [JsonProperty("number_of_vertical_frames")]
    public int? NumberOfVerticalFrames;

    [JsonProperty("number_of_horizontal_frames")]
    public int? NumberOfHorizontalFrames;

    [JsonProperty("milliseconds_per_frame")]
    public int? MillisecondsPerFrame;

    [JsonProperty("apply_original_size")]
    public bool? ApplyOriginalSize;
}