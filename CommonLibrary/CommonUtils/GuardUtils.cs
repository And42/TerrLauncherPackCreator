using System;

namespace CommonLibrary.CommonUtils;

public static class GuardUtils
{
    public static T AssertNotNull<T>(this T? self) where T : class
    {
        if (self is null)
            throw new NullReferenceException("null value provided while was not expected");

        return self;
    }
}