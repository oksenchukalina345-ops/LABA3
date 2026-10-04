using System;
using System.Collections.Generic;

namespace LABA3.Models
{
    public sealed class LinearInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points, double x)
        {
            int i = FindIntervalIndex(points, x);
            PointD p1 = points[i];
            PointD p2 = points[i + 1];

            if (Math.Abs(p2.X - p1.X) < 1e-12)
                throw new InvalidOperationException("Точки не можуть мати однакові X.");

            double t = (x - p1.X) / (p2.X - p1.X);

            return p1.Y + t * (p2.Y - p1.Y);
        }
    }
}