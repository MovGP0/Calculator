using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Calculator.Styling
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    public sealed class ColorToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value) {
                return null;
            }
            
            // ReSharper disable once InvertIf
            if (value is Color) {
                var color = (Color)value;
                return new SolidColorBrush(color);
            }
            
            throw new InvalidOperationException($"Type {value.GetType()} cannot be converted.");            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
