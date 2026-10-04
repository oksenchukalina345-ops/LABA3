using System;
using System.Collections.Generic;

namespace LABA3.Models
{
    public sealed class HermiteInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points, double x)
        {
            int i = FindIntervalIndex(points, x);
            PointD p0 = points[i];
            PointD p1 = points[i + 1];

            double dx = p1.X - p0.X;
            if (Math.Abs(dx) < 1e-12)
                throw new InvalidOperationException("Точки не можуть мати однакові X.");

            double t = (x - p0.X) / dx;

            double m0 = CalculateDerivative(points, i);
            double m1 = CalculateDerivative(points, i + 1);

            double h00 = 2 * Math.Pow(t, 3) - 3 * Math.Pow(t, 2) + 1;
            double h10 = Math.Pow(t, 3) - 2 * Math.Pow(t, 2) + t;
            double h01 = -2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2);
            double h11 = Math.Pow(t, 3) - Math.Pow(t, 2);

            return h00 * p0.Y + h10 * dx * m0 + h01 * p1.Y + h11 * dx * m1;
        }

        private static double CalculateDerivative(IReadOnlyList<PointD> points, int index)
        {
            int n = points.Count;
            if (index == 0)
                return (points[1].Y - points[0].Y) / (points[1].X - points[0].X);

            if (index == n - 1)
                return (points[n - 1].Y - points[n - 2].Y) / (points[n - 1].X - points[n - 2].X);

            return (points[index + 1].Y - points[index - 1].Y) / (points[index + 1].X - points[index - 1].X);
        }
    }
}