using System.ComponentModel;
using System.Windows;

namespace TerrLauncherPackCreator.Code.Utils;

public static class DesignerUtils
{
    public static bool IsInDesignMode()
    {
        return DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}