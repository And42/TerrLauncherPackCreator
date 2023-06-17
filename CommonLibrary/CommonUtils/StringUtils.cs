using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CommonLibrary.CommonUtils;

public static class StringUtils
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
    
    public static string JoinToString(this IEnumerable<string?> parts, string? separator)
    {
        return string.Join(separator, parts);
    }
    
    public static string JoinToString(this IEnumerable<string?> parts, char separator)
    {
        return string.Join(separator, parts);
    }
}