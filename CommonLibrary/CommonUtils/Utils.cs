using System;

namespace CommonLibrary.CommonUtils;

internal static class Utils
{
    public static T AssertNotNull<T>(this T? self) where T : class
    {
        if (self is null)
            throw new NullReferenceException("null value provided while was not expected");

        return self;
    }
}