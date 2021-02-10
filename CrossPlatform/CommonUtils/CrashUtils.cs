using System;

namespace CommonLibrary.CommonUtils
{
    public static class CrashUtils
    {
        public static void HandleException(Exception exception)
        {
#if DEBUG
            throw exception;
#endif
        }
    }
}
