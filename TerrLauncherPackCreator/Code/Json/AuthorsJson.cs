using System;
using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrLauncherPackCreator.Code.Implementations;
using TerrLauncherPackCreator.Code.Utils;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AuthorsJson {

        public static class Processor
        {
            public static AuthorsJson Deserialize(string json) {
                JObject fileJson = JObject.Parse(json);
                int fileVersion = fileJson["version"]?.ToObject<int>() ?? 0;
                bool updatePerformed = false;
                for (; fileVersion < LatestVersion; fileVersion++) {
                    switch (fileVersion) {
                        case 0:
                        {
                            fileJson["version"] = 1;
                            var authors = fileJson["authors"];
                            if (authors != null)
                            {
                                foreach (var author in authors)
                                {
                                    byte[]? iconBytes = author["icon"]?.ToObject<byte[]>();
                                    if (iconBytes != null)
                                    {
                                        author["icon"] = JObject.FromObject(new
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
                                foreach (var author in authors)
                                    author["icon_height"] = PackUtils.DefaultAuthorIconHeight;
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileVersion), fileVersion, @"Unknown version");
                    }

                    updatePerformed = true;
                }

                return updatePerformed
                    ? fileJson.ToObject<AuthorsJson>() ?? throw new Exception("Can't parse updated authors file")
                    : JsonConvert.DeserializeObject<AuthorsJson>(json);
            }

            public static string Serialize(AuthorsJson model) {
                return JsonUtils.Serialize(model);
            }
        }

        private const int LatestVersion = 2;
        
        [JsonProperty("version")]
        public int Version { get; set; }
        
        [JsonProperty("authors")]
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
            [JsonProperty("bytes")]
            public byte[] Bytes { get; set; }
            
            [JsonProperty("type")]
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
        
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("color")]
        public Color? Color { get; set; }

        [JsonProperty("link")]
        public string? Link { get; set; }

        [JsonProperty("icon")]
        public IconJson? Icon { get; set; }
        
        [JsonProperty("icon_height")]
        public int IconHeight { get; set; } = PackUtils.DefaultAuthorIconHeight;

        public AuthorJson() {}
        
        public AuthorJson(
            string name,
            Color? color,
            string link,
            IconJson icon,
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
}