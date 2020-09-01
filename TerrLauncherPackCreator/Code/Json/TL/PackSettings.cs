using System;
using System.Collections.Generic;
using CommonLibrary.CommonUtils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerrLauncherPackCreator.Code.Enums;
using TerrLauncherPackCreator.Code.Utils;
using TerrLauncherPackCreator.Code.ViewModels;

namespace TerrLauncherPackCreator.Code.Json.TL
{
    public class PackSettings
    {
        public static class Processor
        {
            [NotNull]
            public static PackSettings Deserialize([NotNull] string json)
            {
                JObject jsonObject = JObject.Parse(json);
                int packStructureVersion = jsonObject["packStructureVersion"]?.ToObject<int>() ?? 0;
                while (packStructureVersion < PackCreationViewModel.LatestPackStructureVersion)
                {
                    if (packStructureVersion < 15)
                    {
                        var authors = jsonObject["authors"]?.ToObject<string>();
                        if (authors != null)
                        {
                            jsonObject["authors"] = JArray.FromObject(authors
                                .Split(new[] {"<->"}, StringSplitOptions.RemoveEmptyEntries)
                                .ConvertAll(StringToAuthorJson)
                            );
                        }

                        packStructureVersion = 16;
                    }
                    else
                    {
                        throw new Exception("Can't handle structure version: " + packStructureVersion);
                    }
                }

                return jsonObject.ToObject<PackSettings>() ?? throw new Exception("Can't parse json");
            }

            [NotNull]
            public static string Serialize([NotNull] PackSettings settings)
            {
                return JsonUtils.Serialize(settings);
            }
            
            private static AuthorJson StringToAuthorJson(string author)
            {
                string name = null;
                string color = null;
                string link = null;
                string file = null;
                
                string[] parts = author.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string[] keyValue = part.Split('=');
                    if (keyValue.Length != 2)
                        continue;
                
                    switch (keyValue[0])
                    {
                        case "name":
                            name = keyValue[1];
                            break;
                        case "color":
                            color = keyValue[1];
                            break;
                        case "link":
                            link = keyValue[1];
                            break;
                        case "file":
                            file = keyValue[1];
                            break;
                    }
                }
            
                return new AuthorJson(
                    name: name,
                    color: color,
                    file: file,
                    link: link,
                    iconHeight: PackUtils.DefaultAuthorIconHeight
                );
            }
        }

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
        public List<AuthorJson> Authors { get; set; }

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
            [CanBeNull] List<AuthorJson> authors,
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