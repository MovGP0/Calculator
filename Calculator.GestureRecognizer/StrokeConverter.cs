using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Ink;
using InkStroke = System.Windows.Ink.Stroke;

namespace Calculator.GestureRecognizer
{
    public sealed class StrokeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(IEnumerable<Stroke>) 
                || sourceType == typeof(IEnumerable<InkStroke>) 
                || sourceType == typeof(StrokeCollection)
                || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(IEnumerable<Stroke>) 
                || destinationType == typeof(IEnumerable<InkStroke>) 
                || destinationType == typeof(StrokeCollection)
                || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
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

            return base.ConvertFrom(context, culture, value);
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
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null) return null;

            var strokeCollection = value as StrokeCollection;
            if (strokeCollection == null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            
            if (destinationType == typeof(StrokeCollection))
            {
                return strokeCollection;
            }

            if(destinationType == typeof(IEnumerable<Stroke>))
            {
                return strokeCollection.ConvertToStrokes();
            }

            if(destinationType == typeof(IEnumerable<InkStroke>))
            {
                return strokeCollection.ConvertToInkStrokes();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}