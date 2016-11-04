using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Calculator.GestureRecognizer
{
    public static class StrokeExtensions
    {
        public static IEnumerable<Stroke> TranslateTo(this IEnumerable<Stroke> strokes, Point p)
        {
            return strokes.Select(s => new Stroke(s.Points.TranslateTo(p)));
        }

        public static IEnumerable<Stroke> Scale(this IEnumerable<Stroke> strokes)
        {
            var strokesArray = strokes.ToArray();
            var minx = double.MaxValue;
            var miny = double.MaxValue;
            var maxx = double.MinValue;
            var maxy = double.MinValue;

            foreach (var point in strokesArray.SelectMany(s => s.Points))
            {
                if (minx > point.X) minx = point.X;
                if (miny > point.Y) miny = point.Y;
                if (maxx < point.X) maxx = point.X;
                if (maxy < point.Y) maxy = point.Y;
            }

            var scale = Math.Max(maxx - minx, maxy - miny);

            foreach (var stroke in strokesArray)
            {
                yield return new Stroke(stroke.Points.Scale(minx, miny, scale));
            }
        }
        
        public static Point Controid(this IEnumerable<Stroke> strokes)
        {
            return strokes.SelectMany(s => s.Points).Centroid();
        }
        
        private static IEnumerable<Point> Scale(this IEnumerable<Point> points, double minx, double miny, double scale)
        {
            return points.Select(point => new Point { X = (point.X - minx)/scale, Y = (point.Y - miny)/scale });
        }

        public static IEnumerable<Stroke> Resample(this IEnumerable<Stroke> strokes, int samplingResolution)
        {
            return strokes.Select(s => Resample(s, samplingResolution));
        }

        private static Stroke Resample(Stroke stroke, int samplingResolution)
        {
            var resampledPoints = stroke.Points.Resample(samplingResolution);
            return new Stroke(resampledPoints);
        }
    }
}