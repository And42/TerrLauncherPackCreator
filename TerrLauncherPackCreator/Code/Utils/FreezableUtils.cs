using System.Windows;

namespace TerrLauncherPackCreator.Code.Utils
{
    public static class FreezableUtils
    {
        public static T FreezeIfCan<T>(this T obj) where T : Freezable
        {
            if (obj.CanFreeze)
                obj.Freeze();

            return obj;
        }
    }
}
