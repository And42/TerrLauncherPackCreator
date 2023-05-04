using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CrossPlatform.Code.Utils;

public static class JsonUtils
{
    public static string Serialize<T>(T value)
    {
        StringBuilder sb = new(256);
        StringWriter sw = new(sb, CultureInfo.InvariantCulture);

        var jsonSerializer = JsonSerializer.CreateDefault();
        using (JsonTextWriter jsonWriter = new(sw))
        {
            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.IndentChar = ' ';
            jsonWriter.Indentation = 4;

            jsonSerializer.Serialize(jsonWriter, value, typeof(T));
        }

        return sw.ToString();
    }

    public static T Deserialize<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value)
            ?? throw new InvalidOperationException("Unable to deserialize json: " + value);
    }
}