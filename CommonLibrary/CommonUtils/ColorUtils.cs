namespace CommonLibrary.CommonUtils;

public static class ColorUtils
{
    public static System.Windows.Media.Color ToMediaColor(this in System.Drawing.Color color)
    {
        return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
        
    public static System.Drawing.Color ToDrawingColor(this in System.Windows.Media.Color color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}