using CrossPlatform.Code.Utils;
using Newtonsoft.Json;

namespace CrossPlatform.Code.Json.TL
{
    public class AuthorJson
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("color")]
        public string? Color { get; set; }
        
        [JsonProperty("file")]
        public string? File { get; set; }
        
        [JsonProperty("link")]
        public string? Link { get; set; }
        
        [JsonProperty("icon_height")]
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
}