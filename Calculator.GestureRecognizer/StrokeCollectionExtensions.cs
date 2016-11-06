using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace Calculator.GestureRecognizer
{
    internal static class InkStrokeExtensions
    {
        public static IEnumerable<Stroke> ConvertToStrokes(this StrokeCollection collection)
        {
            return collection.Select(s => s.ConvertToStroke());
        }
    
        private static Stroke ConvertToStroke(this System.Windows.Ink.Stroke stroke)
        {
            return new Stroke(stroke.StylusPoints.ConvertToPoints());
        }
    
        private static IEnumerable<Point> ConvertToPoints(this StylusPointCollection points)
        {
            return points.Select(p => p.ConvertToPoint());
        }

        private static Point ConvertToPoint(this StylusPoint point)
        {
            return new Point(point.X, point.Y);
        }
    }
}