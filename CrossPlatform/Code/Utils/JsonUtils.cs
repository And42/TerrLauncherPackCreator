using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CrossPlatform.Code.Utils;

public static class JsonUtils
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        IncludeFields = true
    };
    
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, SerializerOptions);
    }

    public static JsonNode? SerializeToNode<TValue>(TValue value)
    {
        return JsonSerializer.SerializeToNode(value, SerializerOptions);
    }

    public static T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, SerializerOptions)
            ?? throw new InvalidOperationException("Unable to deserialize json: " + value);
    }

    public static T? Deserialize<T>(JsonNode node)
    {
        return node.Deserialize<T>(SerializerOptions);
    }

    public static JsonNode? ParseJsonNode(string json)
    {
        return JsonNode.Parse(json);
    }
}