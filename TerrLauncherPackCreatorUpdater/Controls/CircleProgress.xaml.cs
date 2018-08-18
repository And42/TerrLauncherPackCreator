using System.Windows;

namespace TerrLauncherPackCreatorUpdater.Controls
{
    public partial class CircleProgress
    {
        public static readonly DependencyProperty PrimaryProgressProperty = DependencyProperty.Register(
            "PrimaryProgress", typeof(int), typeof(CircleProgress), new PropertyMetadata(50));

        public int PrimaryProgress
        {
            get => (int) GetValue(PrimaryProgressProperty);
            set => SetValue(PrimaryProgressProperty, value);
        }

        public static readonly DependencyProperty SecondaryProgressProperty = DependencyProperty.Register(
            "SecondaryProgress", typeof(int), typeof(CircleProgress), new PropertyMetadata(default(int)));

        public int SecondaryProgress
        {
            get => (int) GetValue(SecondaryProgressProperty);
            set => SetValue(SecondaryProgressProperty, value);
        }

        public static readonly DependencyProperty DownloadSpeedInBytesProperty = DependencyProperty.Register(
            "DownloadSpeedInBytes", typeof(long), typeof(CircleProgress), new PropertyMetadata(default(long)));

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
}
