using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Laba3
{
    public partial class MainWindow : Window
    {
        private List<Point> _a1Points = new List<Point>(); private double _xMin = 0; private double _xMax = 2 * Math.PI;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateBasePoints();
            DrawAll();
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawAll();
        }

        private void GenerateBasePoints()
        {
            _a1Points.Clear();
            double h1 = 1.0;
            for (double x = _xMin; x <= _xMax + 1e-5; x += h1)
            {
                _a1Points.Add(new Point(x, Math.Sin(x)));
            }
        }

        private void DrawAll()
        {
            if (MainCanvas == null || MainCanvas.ActualWidth == 0 || MainCanvas.ActualHeight == 0)
                return;

            MainCanvas.Children.Clear();

            double width = MainCanvas.ActualWidth;
            double height = MainCanvas.ActualHeight;

            Point ToCanvas(Point p)
            {
                double cx = (p.X - _xMin) / (_xMax - _xMin) * (width - 60) + 30;
                double cy = height / 2 - (p.Y * (height / 3));
                return new Point(cx, cy);
            }

            // Вісь X
            Line xAxis = new Line
            {
                X1 = 0,
                Y1 = height / 2,
                X2 = width,
                Y2 = height / 2,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1
            };
            MainCanvas.Children.Add(xAxis);

            double h2 = 0.02;
            int steps = (int)((_xMax - _xMin) / h2);

            Interpolation linearCalc = new LinearInterpolation();
            Interpolation hermiteCalc = new HermiteInterpolation();

            Polyline exactLine = new Polyline { Stroke = Brushes.Gray, StrokeThickness = 1.5, StrokeDashArray = new DoubleCollection { 4, 2 } };
            Polyline linearLine = new Polyline { Stroke = Brushes.Red, StrokeThickness = 2 };
            Polyline hermiteLine = new Polyline { Stroke = Brushes.Blue, StrokeThickness = 2 };

            for (int i = 0; i <= steps; i++)
            {
                double x = _xMin + i * h2;
                if (x > _xMax) x = _xMax;

                exactLine.Points.Add(ToCanvas(new Point(x, Math.Sin(x))));
                linearLine.Points.Add(ToCanvas(new Point(x, linearCalc.Calculate(_a1Points, x))));
                hermiteLine.Points.Add(ToCanvas(new Point(x, hermiteCalc.Calculate(_a1Points, x))));
            }

            MainCanvas.Children.Add(exactLine);
            MainCanvas.Children.Add(linearLine);
            MainCanvas.Children.Add(hermiteLine);

            // Малювання точок A1
            foreach (var pt in _a1Points)
            {
                Point cp = ToCanvas(pt);
                Ellipse ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.Black,
                    Margin = new Thickness(cp.X - 4, cp.Y - 4, 0, 0)
                };
                MainCanvas.Children.Add(ellipse);
            }
        }
    }
}