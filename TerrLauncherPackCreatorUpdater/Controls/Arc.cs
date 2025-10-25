using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TerrLauncherPackCreatorUpdater.Controls;

public class Arc : Shape
{
    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(Arc),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(Arc),
            new FrameworkPropertyMetadata(90.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double EndAngle
    {
        get => (double)GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            double startAngle = StartAngle - 90;
            double endAngle = EndAngle - 90;
            double radiusX = ActualWidth / 2;
            double radiusY = ActualHeight / 2;
            Point center = new Point(radiusX, radiusY);

            Point startPoint = new Point(
                center.X + radiusX * Math.Cos(startAngle * Math.PI / 180),
                center.Y + radiusY * Math.Sin(startAngle * Math.PI / 180));

            Point endPoint = new Point(
                center.X + radiusX * Math.Cos(endAngle * Math.PI / 180),
                center.Y + radiusY * Math.Sin(endAngle * Math.PI / 180));

            bool isLargeArc = Math.Abs(endAngle - startAngle) > 180;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = startPoint };
            figure.Segments.Add(new ArcSegment(endPoint, new Size(radiusX, radiusY), 0, isLargeArc, SweepDirection.Clockwise, true));
            geometry.Figures.Add(figure);

            return geometry;
        }
    }
}
