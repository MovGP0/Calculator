using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Ink;
using InkStroke = System.Windows.Ink.Stroke;

namespace Calculator.GestureRecognizer
{
    [ValueConversion(typeof(IEnumerable<InkStroke>), typeof(StrokeCollection))]
    [ValueConversion(typeof(IEnumerable<Stroke>), typeof(StrokeCollection))]
    [ValueConversion(typeof(StrokeCollection), typeof(StrokeCollection))]
    
    [ValueConversion(typeof(StrokeCollection), typeof(IEnumerable<InkStroke>))]
    [ValueConversion(typeof(StrokeCollection), typeof(IEnumerable<Stroke>))]
    [ValueConversion(typeof(StrokeCollection), typeof(StrokeCollection))]
    public sealed class StrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if(value is StrokeCollection)
                return value;
            
            StrokeCollection strokeCollection;
            if (TryConvertInkStroke(value, out strokeCollection))
                return strokeCollection;

            if (TryConvertStrokes(value, out strokeCollection))
                return strokeCollection;
            
            throw new NotSupportedException($"Cannot convert to type {targetType}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var strokeCollection = value as StrokeCollection;
            if (strokeCollection == null)
            {
                throw new NotSupportedException($"Cannot convert from type {value.GetType()}");
            }

            if (targetType == typeof(StrokeCollection))
            {
                return strokeCollection;
            }

            if(targetType == typeof(IEnumerable<Stroke>))
            {
                return strokeCollection.ConvertToStrokes();
            }

            if(targetType == typeof(IEnumerable<InkStroke>))
            {
                return strokeCollection.ConvertToInkStrokes();
            }
            
            throw new NotSupportedException($"Cannot convert to type {targetType}");
        }

        private static bool TryConvertInkStroke(object value, out StrokeCollection strokeCollection)
        {
            var strokes = value as IEnumerable<InkStroke>;
            if (strokes != null)
            {
                strokeCollection = new StrokeCollection(strokes);
                return true;
            }
            
            strokeCollection = null;
            return false;
        }

        private static bool TryConvertStrokes(object value, out StrokeCollection strokeCollection)
        {
            var strokes2 = value as IEnumerable<Stroke>;
            if (strokes2 != null)
            {
                strokeCollection = new StrokeCollection(strokes2.ConvertToInkStrokes());
                return true;
            }
            
            strokeCollection = null;
            return false;
        }
    }
}