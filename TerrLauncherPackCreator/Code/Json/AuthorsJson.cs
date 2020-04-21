using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Implementations;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AuthorsJson {
        public const int LatestVersion = 1;
        
        [JsonProperty("version")]
        public int Version { get; set; }
        
        [CanBeNull]
        [JsonProperty("authors")]
        public List<AuthorJson> Authors { get; set; }

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
        }
        
        [CanBeNull]
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [CanBeNull]
        [JsonProperty("color")]
        public Color? Color { get; set; }

        [CanBeNull]
        [JsonProperty("link")]
        public string Link { get; set; }

        [CanBeNull]
        [JsonProperty("icon")]
        public IconJson Icon { get; set; }
    }
}