using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows.Media;
using CrossPlatform.Code.Implementations;
using CrossPlatform.Code.Utils;

namespace TerrLauncherPackCreator.Code.Json;

public class AuthorsJson {

    public static class Processor
    {
        public static AuthorsJson Deserialize(string json) {
            JsonNode fileJson = JsonUtils.ParseJsonNode(json) ?? throw new Exception("Can't parse authors file");
            int fileVersion = fileJson["version"]?.GetValue<int>() ?? 0;
            bool updatePerformed = false;
            for (; fileVersion < LatestVersion; fileVersion++) {
                switch (fileVersion) {
                    case 0:
                    {
                        fileJson["version"] = 1;
                        JsonNode? authors = fileJson["authors"];
                        if (authors != null)
                        {
                            foreach (JsonNode? author in authors.AsArray())
                            {
                                if (author == null)
                                    continue;
                                
                                byte[]? iconBytes = author["icon"]?.GetValue<byte[]>();
                                if (iconBytes != null)
                                {
                                    author["icon"] = JsonUtils.SerializeToNode(new
                                    {
                                        bytes = iconBytes,
                                        type = ImageInfo.ImageType.Png
                                    });
                                }
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        fileJson["version"] = 2;
                        var authors = fileJson["authors"];
                        if (authors != null)
                        {
                            foreach (var author in authors.AsArray())
                            {
                                if (author != null)
                                {
                                    author["icon_height"] = PackUtils.DefaultAuthorIconHeight;
                                }
                            }
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fileVersion), fileVersion, @"Unknown version");
                }

                updatePerformed = true;
            }

            return updatePerformed
                ? JsonUtils.Deserialize<AuthorsJson>(fileJson) ?? throw new Exception("Can't parse updated authors file")
                : JsonUtils.Deserialize<AuthorsJson>(json);
        }

        public static string Serialize(AuthorsJson model) {
            return JsonUtils.Serialize(model);
        }
    }

    private const int LatestVersion = 2;
        
    [JsonPropertyName("version")]
    public int Version { get; set; }
        
    [JsonPropertyName("authors")]
    public List<AuthorJson>? Authors { get; set; }

    public static AuthorsJson CreateLatest() {
        return new AuthorsJson {
            Version = LatestVersion
        };
    }
}

public class AuthorJson
{
    public class IconJson {
        [JsonPropertyName("bytes")]
        public byte[] Bytes { get; set; } = null!;

        [JsonPropertyName("type")]
        public ImageInfo.ImageType Type { get; set; }
            
        public IconJson() {}

        public IconJson(
            byte[] bytes,
            ImageInfo.ImageType type
        )
        {
            Bytes = bytes;
            Type = type;
        }
    }
        
    [JsonPropertyName("name")]
    public string? Name { get; set; }
        
    [JsonPropertyName("color")]
    public Color? Color { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("icon")]
    public IconJson? Icon { get; set; }
        
    [JsonPropertyName("icon_height")]
    public int IconHeight { get; set; } = PackUtils.DefaultAuthorIconHeight;

    public AuthorJson() {}
        
    public AuthorJson(
        string? name,
        Color? color,
        string? link,
        IconJson? icon,
        int iconHeight
    )
    {
        Name = name;
        Color = color;
        Link = link;
        Icon = icon;
        IconHeight = iconHeight;
    }
}