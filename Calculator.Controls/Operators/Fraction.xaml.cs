using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls.Operators
{
    public sealed partial class Fraction
    {
        #region DependencyProperties
        private static readonly DependencyProperty NumeratorProperty = DependencyProperty.Register(nameof(Numerator), typeof(UIElement), typeof(Fraction), new FrameworkPropertyMetadata(default(UIElement)) {AffectsMeasure = true} );
        public UIElement Numerator
        {
            get { return (UIElement)GetValue(NumeratorProperty); }
            set { SetValue(NumeratorProperty, value); }
        }

        private static readonly DependencyProperty DenominatorProperty = DependencyProperty.Register(nameof(Denominator), typeof(UIElement), typeof(Fraction), new FrameworkPropertyMetadata(default(UIElement)) {AffectsMeasure = true});
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

        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(Fraction), new PropertyMetadata(1.0));
        public double LineThickness
        {
            get { return (double) GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        
        public static readonly DependencyProperty NumeratorLeftProperty = DependencyProperty.Register(nameof(NumeratorLeft), typeof(double), typeof(Fraction), new PropertyMetadata(default(double)));
        public double NumeratorLeft
        {
            get { return (double) GetValue(NumeratorLeftProperty); }
            set { SetValue(NumeratorLeftProperty, value); }
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
            var lineHeight = FontSize/10.0;
            var linePadding = lineHeight;

            var height = numeratorHeight + linePadding + lineHeight + linePadding + denominatorHeight;
            var width = Math.Max(numeratorWidth, denominatorWidth);
            
            BaselineOffset = CalculateBaseline(numeratorHeight - lineHeight, FontSize, FontFamily);
            LineThickness = lineHeight;
            
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
            var lineHeight = LineThickness;
            var linePadding = lineHeight;

            var maxWidth = Math.Max(numeratorWidth, denominatorWidth);

            NumeratorLeft = (maxWidth-numeratorWidth)/2.0;
            DenominatorTop = numeratorHeight + linePadding + lineHeight + linePadding;
            DenominatorLeft = (maxWidth-denominatorWidth)/2.0;
            
            var width = Math.Max(denominatorWidth, numeratorWidth);
            var height = numeratorHeight + linePadding + lineHeight + linePadding + denominatorHeight;

            base.ArrangeOverride(new Size(width, height));

            var lineY = numeratorHeight + linePadding + lineHeight/2.0;
            _leftPoint = new Point(0, lineY);
            _rightPoint = new Point(width, lineY);

            return new Size(width, height);
        }

        private Point _leftPoint;
        private Point _rightPoint;

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawLine(new Pen(Foreground, LineThickness), _leftPoint, _rightPoint);
        }
    }
}
