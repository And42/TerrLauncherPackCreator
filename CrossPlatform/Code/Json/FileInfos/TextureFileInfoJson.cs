using System.Text.Json.Serialization;
using CrossPlatform.Code.FileInfos;

namespace CrossPlatform.Code.Json.FileInfos;

internal class TextureFileInfoJson
{
    [JsonPropertyName("type")]
    public TextureFileInfo.TextureType Type = TextureFileInfo.TextureType.General;

    [JsonPropertyName("entry_name")]
    public string? EntryName;

    [JsonPropertyName("element_id")]
    public int? ElementId;

    [JsonPropertyName("animated")]
    public bool? Animated;

    [JsonPropertyName("animate_in_gui")]
    public bool? AnimateInGui;

    [JsonPropertyName("number_of_vertical_frames")]
    public int? NumberOfVerticalFrames;

    [JsonPropertyName("number_of_horizontal_frames")]
    public int? NumberOfHorizontalFrames;

    [JsonPropertyName("milliseconds_per_frame")]
    public int? MillisecondsPerFrame;

    [JsonPropertyName("apply_original_size")]
    public bool? ApplyOriginalSize;
}