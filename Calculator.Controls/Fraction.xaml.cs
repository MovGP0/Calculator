using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls
{
    public sealed partial class Fraction
    {
        #region DependencyProperties
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

        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register(nameof(LineHeight), typeof(double), typeof(Fraction), new PropertyMetadata(1.0));
        public double LineHeight
        {
            get { return (double) GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register(nameof(LineWidth), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double LineWidth
        {
            get { return (double) GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
        
        public static readonly DependencyProperty NumeratorLeftProperty = DependencyProperty.Register(nameof(NumeratorLeft), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double NumeratorLeft
        {
            get { return (double) GetValue(NumeratorLeftProperty); }
            set { SetValue(NumeratorLeftProperty, value); }
        }

        public static readonly DependencyProperty LineTopProperty = DependencyProperty.Register(nameof(LineTop), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double LineTop
        {
            get { return (double) GetValue(LineTopProperty); }
            set { SetValue(LineTopProperty, value); }
        }

        public static readonly DependencyProperty DenominatorLeftProperty = DependencyProperty.Register(nameof(DenominatorLeft), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double DenominatorLeft
        {
            get { return (double) GetValue(DenominatorLeftProperty); }
            set { SetValue(DenominatorLeftProperty, value); }
        }

        public static readonly DependencyProperty DenominatorTopProperty = DependencyProperty.Register(nameof(DenominatorTop), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double DenominatorTop
        {
            get { return (double) GetValue(DenominatorTopProperty); }
            set { SetValue(DenominatorTopProperty, value); }
        }
        #endregion

        public Fraction()
        {
            InitializeComponent();
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            var childSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Numerator?.Measure(childSize);
            Denominator?.Measure(childSize);
            
            var numerator = Numerator;
            var denominator = Denominator;

            var numeratorHeight = numerator?.DesiredSize.Height ?? 0d;
            var numeratorWidth = numerator?.DesiredSize.Width ?? 0d;
            var denominatorHeight = denominator?.DesiredSize.Height ?? 0d;
            var denominatorWidth = denominator?.DesiredSize.Width ?? 0d;

            var height = numeratorHeight + denominatorHeight + 3d;
            var width = Math.Max(numeratorWidth, denominatorWidth);
            
            BaselineOffset = CalculateBaseline(numeratorHeight, FontSize, FontFamily);

            return new Size(width, height);
        }
        
        private static double CalculateBaseline(double numeratorHeight, double fontSize, FontFamily fontFamily)
        {
            var tb = new TextBlock { Text = "Fg", FontSize = fontSize, FontFamily = fontFamily };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return tb.BaselineOffset/2d + numeratorHeight;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var numeratorHeight = Numerator?.DesiredSize.Height ?? 0.0;
            var numeratorWidth = Numerator?.DesiredSize.Width ?? 0.0;
            var denominatorHeight = Denominator?.DesiredSize.Height ?? 0.0d;
            var denominatorWidth = Denominator?.DesiredSize.Width ?? 0.0d;
            var lineHeight = LineHeight;

            var height = numeratorHeight + lineHeight + denominatorHeight;
            var width = Math.Max(numeratorWidth, denominatorWidth);

            var numeratorLeft = (width - numeratorWidth)/2.0;
            var lineBottom = numeratorHeight + lineHeight;
            var denominatorLeft = (width - denominatorWidth)/2.0;

            LineWidth = width;
            NumeratorLeft = numeratorLeft;
            LineTop = numeratorHeight + 2;
            DenominatorTop = lineBottom + 4;
            DenominatorLeft = denominatorLeft;

            return base.ArrangeOverride(new Size(width, height));
        }
    }
}
