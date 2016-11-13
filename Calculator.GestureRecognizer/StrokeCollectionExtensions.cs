using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using InkStroke = System.Windows.Ink.Stroke;

namespace Calculator.GestureRecognizer
{
    public static class InkStrokeExtensions
    {
        public static IEnumerable<Stroke> ConvertToStrokes(this StrokeCollection collection)
        {
            return collection.Select(s => s.ConvertToStroke());
        }

        public static IEnumerable<InkStroke> ConvertToInkStrokes(this StrokeCollection collection)
        {
            return collection.Select(s => s);
        }
    
        public static Stroke ConvertToStroke(this InkStroke stroke)
        {
            return new Stroke(stroke.StylusPoints.ConvertToPoints());
        }
    
        public static InkStroke ConvertToInkStroke(this Stroke stroke)
        {
            return new InkStroke(new StylusPointCollection(stroke.Points));
        }

        public static IEnumerable<InkStroke> ConvertToInkStrokes(this IEnumerable<Stroke> strokes)
        {
            return strokes.Select(s => s.ConvertToInkStroke());
        }

        public static StrokeCollection ConvertToStrokeCollection(this IEnumerable<Stroke> strokes)
        {
            return new StrokeCollection(strokes.ConvertToInkStrokes());
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