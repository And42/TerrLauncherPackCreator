using System.Diagnostics.CodeAnalysis;

namespace CommonLibrary.CommonUtils;

public static class StringUtils
{
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
}