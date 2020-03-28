using System.Collections.Generic;
using System.Windows.Media;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TerrLauncherPackCreator.Code.Json
{
    public class AuthorsJson
    {
        [CanBeNull]
        [JsonProperty("authors")]
        public List<AuthorJson> Authors { get; set; }
    }

    public class AuthorJson
    {
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
        public byte[] Icon { get; set; }
    }
}