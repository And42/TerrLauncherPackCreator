using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TerrLauncherPackCreator.Code.Enums;

namespace TerrLauncherPackCreator.Code.Json
{
    public class PackSettings
    {
        // deprecated
        // [JsonProperty("terrariaStructureVersion")]
        // public int TerrariaStructureVersion { get; set; }

        [JsonProperty("packStructureVersion")]
        public int PackStructureVersion { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [CanBeNull]
        [JsonProperty("descriptionEnglish")]
        public string DescriptionEnglish { get; set; }
        
        [CanBeNull]
        [JsonProperty("descriptionRussian")]
        public string DescriptionRussian { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
        
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
        
        [CanBeNull]
        [JsonProperty("authors")]
        public string Authors { get; set; }

        [CanBeNull]
        [JsonProperty("predefined_tags")]
        public List<PredefinedPackTag> PredefinedTags { get; set; }

        [JsonProperty("is_bonus")]
        public bool IsBonus { get; set; }

        [JsonProperty("bonus_type")]
        public BonusType BonusType { get; set; }
        
        public PackSettings() {}

        public PackSettings(
            int packStructureVersion,
            [CanBeNull] string title,
            [CanBeNull] string descriptionEnglish,
            [CanBeNull] string descriptionRussian,
            int version,
            Guid guid,
            [CanBeNull] string authors,
            [CanBeNull] List<PredefinedPackTag> predefinedTags,
            bool isBonus,
            BonusType bonusType
        )
        {
            PackStructureVersion = packStructureVersion;
            Title = title;
            DescriptionEnglish = descriptionEnglish;
            DescriptionRussian = descriptionRussian;
            Version = version;
            Guid = guid;
            Authors = authors;
            PredefinedTags = predefinedTags;
            IsBonus = isBonus;
            BonusType = bonusType;
        }
    }
}