using System;
using System.Windows;
using System.Windows.Media;

namespace Calculator.Controls
{
    public partial class Pow
    {
        #region Dependent Properties
        public static readonly DependencyProperty ExponentProperty = DependencyProperty.Register(nameof(Exponent), typeof(UIElement), typeof(Pow), new PropertyMetadata(default(UIElement)));

        public UIElement Exponent
        {
            get { return (UIElement) GetValue(ExponentProperty); }
            set { SetValue(ExponentProperty, value); }
        }

        public static readonly DependencyProperty ContentYProperty = DependencyProperty.Register(
            nameof(ContentY), typeof(double), typeof(Pow), new PropertyMetadata(default(double)));

        public double ContentY
        {
            get { return (double) GetValue(ContentYProperty); }
            set { SetValue(ContentYProperty, value); }
        }

        public static readonly DependencyProperty ExponentXProperty = DependencyProperty.Register(
            nameof(ExponentX), typeof(double), typeof(Pow), new PropertyMetadata(default(double)));

        public double ExponentX
        {
            get { return (double) GetValue(ExponentXProperty); }
            set { SetValue(ExponentXProperty, value); }
        }

        public static readonly DependencyProperty ExponentTransformProperty = DependencyProperty.Register(
            nameof(ExponentTransform), typeof(Transform), typeof(Pow), new PropertyMetadata(default(Transform)));

        public Transform ExponentTransform
        {
            get { return (Transform) GetValue(ExponentTransformProperty); }
            set { SetValue(ExponentTransformProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Pow), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }
        #endregion

        public Pow()
        {
            InitializeComponent();
        }

        private const double Scale = 0.8;

        protected override Size MeasureOverride(Size constraint)
        {
            var exponent = Exponent;
            var content = Content as UIElement;

            exponent?.Measure(constraint);
            content?.Measure(constraint);

            var exponentHeight = exponent?.DesiredSize.Height ?? 0.0 * Scale;
            var exponentWidth = exponent?.DesiredSize.Width ?? 0.0 * Scale;
            var contentHeight = content?.DesiredSize.Height ?? 0.0;
            var contentWidth  = content?.DesiredSize.Width ?? 0.0;

            var midlineOffset = Math.Max(contentHeight/2.0, exponentHeight);

            var height = Padding.Top + FontSize / 5.0 + midlineOffset + contentHeight/2.0 + Padding.Bottom;
            var width = Padding.Left + contentWidth + exponentWidth + Padding.Right;

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var exponent = Exponent;
            var content = Content as UIElement;
            
            var exponentHeight = exponent?.DesiredSize.Height ?? 0.0 * Scale;
            var exponentWidth = exponent?.DesiredSize.Width ?? 0.0 * Scale;
            var contentHeight = content?.DesiredSize.Height ?? 0.0;
            var contentWidth  = content?.DesiredSize.Width ?? 0.0;

            var midlineOffset = Math.Max(contentHeight/2.0, exponentHeight);

            var height = Padding.Top + FontSize / 5.0 + midlineOffset + contentHeight/2.0 + Padding.Bottom;
            var width = Padding.Left + contentWidth + exponentWidth + Padding.Right;
            
            base.ArrangeOverride(arrangeBounds);

            ExponentTransform = new ScaleTransform(Scale, Scale, 0, 0);
            ExponentX = contentWidth;
            ContentY = Padding.Top + Math.Min(exponentHeight - midlineOffset, 0.0) + FontSize / 5.0;
            BaselineOffset = content.GetBaselineOffset();

            return new Size(width, height);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Orange, FontSize/10.0), new Rect(new Size(base.ActualWidth, base.ActualHeight)));
        }
    }
}
