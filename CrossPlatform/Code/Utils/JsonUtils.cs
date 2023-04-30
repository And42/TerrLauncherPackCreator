using System;
using System.Text.Json;

namespace CrossPlatform.Code.Utils;

public static class JsonUtils
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, JsonSerializerOptions);
    }

    public static T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value)
            ?? throw new InvalidOperationException("Unable to deserialize: " + value);
    }
}