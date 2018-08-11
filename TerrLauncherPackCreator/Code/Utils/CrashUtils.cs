using System;

namespace TerrLauncherPackCreator.Code.Utils
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
