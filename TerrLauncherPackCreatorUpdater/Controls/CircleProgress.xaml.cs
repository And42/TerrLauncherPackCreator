using System.Windows;

namespace TerrLauncherPackCreatorUpdater.Controls;

public partial class CircleProgress
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress), typeof(int), typeof(CircleProgress), new PropertyMetadata(50));

    public int Progress
    {
        get => (int) GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public static readonly DependencyProperty DownloadSpeedInBytesProperty = DependencyProperty.Register(
        nameof(DownloadSpeedInBytes), typeof(long), typeof(CircleProgress), new PropertyMetadata(0));

    public long DownloadSpeedInBytes
    {
        get => (long) GetValue(DownloadSpeedInBytesProperty);
        set => SetValue(DownloadSpeedInBytesProperty, value);
    }

    public CircleProgress()
    {
        InitializeComponent();
    }
}