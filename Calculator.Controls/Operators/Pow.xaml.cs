using System;
using System.Windows;
using System.Windows.Media;
using Serilog;

namespace Calculator.Controls.Operators
{
    public partial class Pow
    {
        #region Dependent Properties

        public static readonly DependencyProperty CanvasHeightProperty = DependencyProperty.Register(
            nameof(CanvasHeight), typeof(double), typeof(Pow), new PropertyMetadata(default(double)));

        public double CanvasHeight
        {
            get { return (double) GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public static readonly DependencyProperty CanvasWidthProperty = DependencyProperty.Register(
            nameof(CanvasWidth), typeof(double), typeof(Pow), new PropertyMetadata(default(double)));

        public double CanvasWidth
        {
            get { return (double) GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

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
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        private const double Scale = 0.5;

        protected override Size MeasureOverride(Size constraint)
        {
            var exponent = Exponent;
            var content = Content as UIElement;

            exponent?.Measure(constraint);
            content?.Measure(constraint);

            var exponentHeight = (exponent?.DesiredSize.Height ?? 0.0) * Scale;
            var exponentWidth = (exponent?.DesiredSize.Width ?? 0.0) * Scale;
            var contentHeight = content?.DesiredSize.Height ?? 0.0;
            var contentWidth  = content?.DesiredSize.Width ?? 0.0;

            var midlineOffset = Math.Max(contentHeight/2.0, exponentHeight);

            var canvasWidth = contentWidth + exponentWidth;
            var canvasHeight = midlineOffset + contentHeight/2.0;

            var height = canvasHeight;
            var width = canvasWidth;

            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            var contentOffset = Math.Max(exponentHeight - contentHeight/2d, 0d);
            BaselineOffset = content.GetBaselineOffset() + contentOffset;
            ContentY = contentOffset;

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

            var height = FontSize/5.0 + midlineOffset + contentHeight/2.0;
            var width = contentWidth + exponentWidth;
            
            base.ArrangeOverride(arrangeBounds);

            ExponentTransform = new ScaleTransform(Scale, Scale, 0, 0);
            ExponentX = contentWidth;
            
            return new Size(width, height);
        }
    }
}
