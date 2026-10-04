namespace LABA3.Models
{
    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD()
        {
            X = 0;
            Y = 0;
        }
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}