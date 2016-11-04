using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Calculator.GestureRecognizer
{
    public static class PointExtensions
    {
        public static IEnumerable<Point> TranslateTo(this IEnumerable<Point> points, Point p)
        {
            return points.Select(point => new Point(point.X - p.X, point.Y - p.Y));
        }

        /// <summary>
        /// Resamples the array of points into n equally-distanced points
        /// </summary>
        public static Point[] Resample(this IEnumerable<Point> points, int n)
        {
            var pointArray = points.ToArray();

            var newPoints = new Point[n];
            newPoints[0] = new Point(pointArray[0].X, pointArray[0].Y);
            var numPoints = 1;

            var I = pointArray.PathLength()/(n - 1); // computes interval length
            double D = 0;
            for (var i = 1; i < pointArray.Length; i++)
            {
                var d = Geometry.EuclideanDistance(pointArray[i - 1], pointArray[i]);
                if (D + d >= I)
                {
                    var firstPoint = pointArray.First();
                    while (D + d >= I)
                    {
                        // add interpolated point
                        var t = Math.Min(Math.Max((I - D)/d, 0.0f), 1.0f);
                        if (double.IsNaN(t)) t = 0.5f;
                        newPoints[numPoints++] = new Point(
                            (1.0f - t)*firstPoint.X + t*pointArray[i].X,
                            (1.0f - t)*firstPoint.Y + t*pointArray[i].Y
                        );

                        // update partial length
                        d = D + d - I;
                        D = 0;
                        firstPoint = newPoints[numPoints - 1];
                    }
                    D = d;
                }
                else D += d;
            }
            
            return newPoints.AddLastPointIfMissing(pointArray.Last(), n, numPoints);
        }

        private static Point[] AddLastPointIfMissing(this Point[] newPoints, Point lastPoint, int n, int numPoints)
        {
            if (numPoints == n - 1)
            {
                newPoints[n] = lastPoint;
            }
            return newPoints;
        }

        private static double PathLength(this IEnumerable<Point> points)
        {
            Point? lastPoint = null;
            return points.Aggregate(0d, (length, point) =>
            {
                if (lastPoint == null)
                {
                    lastPoint = point;
                    return length;
                }

                var newLength = length + Geometry.EuclideanDistance(lastPoint.Value, point);
                lastPoint = point;
                return newLength;

            }, l => l);
        }

        public static Point Centroid(this IEnumerable<Point> points)
        {
            var pointsArray = points.ToArray();
            var count = pointsArray.Length;

            var cx = 0d;
            var cy = 0d;
            foreach (var point in pointsArray)
            {
                cx += point.X;
                cy += point.Y;
            }

            return new Point(cx/count, cy/count);
        }

    }
}