using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls
{
    public sealed partial class Fraction
    {
        private static readonly DependencyProperty NumeratorProperty = DependencyProperty.Register(nameof(Numerator), typeof(UIElement), typeof(Fraction), new PropertyMetadata(default(UIElement)));
        public UIElement Numerator
        {
            get { return (UIElement)GetValue(NumeratorProperty); }
            set { SetValue(NumeratorProperty, value); }
        }

        private static readonly DependencyProperty DenominatorProperty = DependencyProperty.Register(nameof(Denominator), typeof(UIElement), typeof(Fraction), new PropertyMetadata(default(UIElement)));
        public UIElement Denominator
        {
            get { return (UIElement)GetValue(DenominatorProperty); }
            set { SetValue(DenominatorProperty, value); }
        }
        
        private static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double BaselineOffset
        {
            get { return (double)GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }
        
        public Fraction()
        {
            InitializeComponent();
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            var numerator = Numerator;
            var denominator = Denominator;

            numerator?.Measure(availableSize);
            denominator?.Measure(availableSize);

            var numeratorHeight = numerator?.DesiredSize.Height ?? 0d;
            BaselineOffset = CalculateBaseline(numeratorHeight, FontSize, FontFamily);
            
            var height = (numerator?.DesiredSize.Height ?? 0d) + (denominator?.DesiredSize.Height ?? 0d) + 3d;
            var width = Math.Max(numerator?.DesiredSize.Width ?? 0d, denominator?.DesiredSize.Width ?? 0d);
            return new Size(Math.Min(width, availableSize.Width), Math.Min(height, availableSize.Height));
        }
        
        private static double CalculateBaseline(double numeratorHeight, double fontSize, FontFamily fontFamily)
        {
            var tb = new TextBlock { Text = "Fg", FontSize = fontSize, FontFamily = fontFamily };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return tb.BaselineOffset/2d + numeratorHeight;
        }
    }
}
