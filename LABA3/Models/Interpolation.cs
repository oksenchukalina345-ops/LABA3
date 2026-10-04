using System;
using System.Collections.Generic;

namespace LABA3.Models
{
  
    public abstract class Interpolation
    {
        public abstract double Calculate(IReadOnlyList<PointD> points, double x);

      
        protected static int FindIntervalIndex(IReadOnlyList<PointD> points, double x)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentException("Для інтерполяції потрібно щонайменше 2 опорні точки.");

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (x >= points[i].X && x <= points[i + 1].X)
                    return i;
            }

            if (x < points[0].X) return 0;
            return points.Count - 2;
        }
    }
}