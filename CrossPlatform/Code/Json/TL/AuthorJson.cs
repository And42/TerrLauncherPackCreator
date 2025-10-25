using System.Text.Json.Serialization;
using CrossPlatform.Code.Utils;

namespace CrossPlatform.Code.Json.TL;

public class AuthorJson
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
        
    [JsonPropertyName("color")]
    public string? Color { get; set; }
        
    [JsonPropertyName("file")]
    public string? File { get; set; }
        
    [JsonPropertyName("link")]
    public string? Link { get; set; }
        
    [JsonPropertyName("icon_height")]
    public int IconHeight { get; set; } = PackUtils.DefaultAuthorIconHeight;
        
    public AuthorJson() {}

    public AuthorJson(
        string? name,
        string? color,
        string? file,
        string? link,
        int iconHeight
    )
    {
        Name = name;
        Color = color;
        File = file;
        Link = link;
        IconHeight = iconHeight;
    }
}