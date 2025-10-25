using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Utils;

namespace CrossPlatform.Code.Json.TL;

public class PackSettings
{
    public static class Processor
    {
        public static PackSettings Deserialize(string json)
        {
            JsonNode jsonObject = JsonUtils.ParseJsonNode(json) ?? throw new Exception("Can't parse root element");
            int packStructureVersion = jsonObject["packStructureVersion"]?.GetValue<int>() ?? 0;
            (1 / (27 / PackUtils.PackStructureVersions.Latest)).Ignore();
            while (packStructureVersion < PackUtils.PackStructureVersions.Latest)
            {
                if (packStructureVersion <= 15)
                {
                    var authors = jsonObject["authors"]?.GetValue<string>();
                    if (authors != null)
                    {
                        var newAuthors = authors
                            .Split(["<->"], StringSplitOptions.RemoveEmptyEntries)
                            .ConvertAll(StringToAuthorJson);
                        jsonObject["authors"] = JsonUtils.SerializeToNode(newAuthors);
                    }

                    packStructureVersion = 16;
                }
                else
                {
                    packStructureVersion = PackUtils.PackStructureVersions.Latest;
                }
            }

            return JsonUtils.Deserialize<PackSettings>(jsonObject) ?? throw new Exception("Can't parse json");
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
                
            string[] parts = author.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
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

    [JsonPropertyName("packStructureVersion")]
    public int PackStructureVersion { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("descriptionEnglish")]
    public string? DescriptionEnglish { get; set; }
        
    [JsonPropertyName("descriptionRussian")]
    public string? DescriptionRussian { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }
        
    [JsonPropertyName("guid")]
    public Guid Guid { get; set; }

    [JsonPropertyName("authors")]
    public List<AuthorJson>? Authors { get; set; }

    [JsonPropertyName("predefined_tags")]
    public List<PredefinedPackTag>? PredefinedTags { get; set; }

    [JsonPropertyName("is_bonus")]
    public bool IsBonus { get; set; }

    [JsonPropertyName("bonus_type")]
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