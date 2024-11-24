using System;
using System.Collections.Generic;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrossPlatform.Code.Json.TL;

public class PackSettings
{
    public static class Processor
    {
        public static PackSettings Deserialize(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            int packStructureVersion = jsonObject["packStructureVersion"]?.ToObject<int>() ?? 0;
            (1 / (27 / PackUtils.PackStructureVersions.Latest)).Ignore();
            while (packStructureVersion < PackUtils.PackStructureVersions.Latest)
            {
                if (packStructureVersion <= 15)
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
                    packStructureVersion = PackUtils.PackStructureVersions.Latest;
                }
            }

            return jsonObject.ToObject<PackSettings>() ?? throw new Exception("Can't parse json");
        }

        public static string Serialize(PackSettings settings)
        {
            return JsonUtils.Serialize(settings);
        }
            
        private static AuthorJson StringToAuthorJson(string author)
        {
            string? name = null;
            string? color = null;
            string? link = null;
            string? file = null;
                
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
    public string Title { get; set; } = null!;

    [JsonProperty("descriptionEnglish")]
    public string? DescriptionEnglish { get; set; }
        
    [JsonProperty("descriptionRussian")]
    public string? DescriptionRussian { get; set; }

    [JsonProperty("version")]
    public int Version { get; set; }
        
    [JsonProperty("guid")]
    public Guid Guid { get; set; }

    [JsonProperty("authors")]
    public List<AuthorJson>? Authors { get; set; }

    [JsonProperty("predefined_tags")]
    public List<PredefinedPackTag>? PredefinedTags { get; set; }

    [JsonProperty("is_bonus")]
    public bool IsBonus { get; set; }

    [JsonProperty("bonus_type")]
    public BonusType BonusType { get; set; }

    public PackSettings() {}

    public PackSettings(
        int packStructureVersion,
        string title,
        string? descriptionEnglish,
        string? descriptionRussian,
        int version,
        Guid guid,
        List<AuthorJson>? authors,
        List<PredefinedPackTag>? predefinedTags,
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