using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Enums;

namespace TerrLauncherPackCreator.Code.Json
{
    public class PackSettings
    {
        [JsonProperty("terrariaStructureVersion")]
        public int TerrariaStructureVersion { get; set; }

        [JsonProperty("packStructureVersion")]
        public int PackStructureVersion { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("descriptionEnglish")]
        public string DescriptionEnglish { get; set; }
        
        [JsonProperty("descriptionRussian")]
        public string DescriptionRussian { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
        
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
        
        [JsonProperty("authors")]
        public string Authors { get; set; }

        [CanBeNull]
        [JsonProperty("predefined_tags")]
        public List<PredefinedPackTag> PredefinedTags { get; set; }
    }
}