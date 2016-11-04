using System;
using System.Windows;

namespace Calculator.GestureRecognizer
{
    public static class Geometry
    {
        public static double SqrEuclideanDistance(Point a, Point b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }
        
        public static double EuclideanDistance(Point a, Point b)
        {
            return Math.Sqrt(SqrEuclideanDistance(a, b));
        }
    }
}
