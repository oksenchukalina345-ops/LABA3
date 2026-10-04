using System;
using System.Collections.Generic;
using System.Windows;
namespace Laba3
{
    public abstract class Interpolation { public abstract double Calculate(IReadOnlyList<Point> points, double x); }
    public sealed class LinearInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<Point> points, double x)
        {
            if (points == null || points.Count == 0) return 0;
            if (x <= points[0].X) return points[0].Y;
            if (x >= points[points.Count - 1].X) return points[points.Count - 1].Y;

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (x >= points[i].X && x <= points[i + 1].X)
                {
                    double x1 = points[i].X, y1 = points[i].Y;
                    double x2 = points[i + 1].X, y2 = points[i + 1].Y;
                    return y1 + (x - x1) / (x2 - x1) * (y2 - y1);
                }
            }
            return 0;
        }
    }

    public sealed class HermiteInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<Point> points, double x)
        {
            int n = points.Count;
            if (n < 2) return 0;
            if (x <= points[0].X) return points[0].Y;
            if (x >= points[n - 1].X) return points[n - 1].Y;

            double[] m = new double[n];
            m[0] = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            m[n - 1] = (points[n - 1].Y - points[n - 2].Y) / (points[n - 1].X - points[n - 2].X);

            for (int i = 1; i < n - 1; i++)
            {
                m[i] = (points[i + 1].Y - points[i - 1].Y) / (points[i + 1].X - points[i - 1].X);
            }

            for (int i = 0; i < n - 1; i++)
            {
                if (x >= points[i].X && x <= points[i + 1].X)
                {
                    double x1 = points[i].X, y1 = points[i].Y;
                    double x2 = points[i + 1].X, y2 = points[i + 1].Y;
                    double dx = x2 - x1;

                    double t = (x - x1) / dx;

                    double h00 = 2 * Math.Pow(t, 3) - 3 * Math.Pow(t, 2) + 1;
                    double h10 = Math.Pow(t, 3) - 2 * Math.Pow(t, 2) + t;
                    double h01 = -2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2);
                    double h11 = Math.Pow(t, 3) - Math.Pow(t, 2);

                    return h00 * y1 + h10 * dx * m[i] + h01 * y2 + h11 * dx * m[i + 1];
                }
            }
            return 0;
        }
    }
}