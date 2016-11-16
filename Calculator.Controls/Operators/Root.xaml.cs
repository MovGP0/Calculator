using System;
using System.Windows;
using System.Windows.Media;
using Serilog;

namespace Calculator.Controls.Operators
{
    public partial class Root
    {
        #region DependencyProperties
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(Root), new PropertyMetadata(default(PointCollection)));
        public PointCollection Points
        {
            get { return (PointCollection) GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty = DependencyProperty.Register(nameof(Index), typeof(UIElement), typeof(Root), new UIPropertyMetadata(default(UIElement)));
        public UIElement Index
        {
            get { return (UIElement) GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        
        public static readonly DependencyProperty IndexTransformProperty = DependencyProperty.Register(nameof(IndexTransform), typeof(Transform), typeof(Root), new PropertyMetadata(default(Transform)));
        public Transform IndexTransform
        {
            get { return (Transform) GetValue(IndexTransformProperty); }
            set { SetValue(IndexTransformProperty, value); }
        }

        public static readonly DependencyProperty ContentXProperty = DependencyProperty.Register(nameof(ContentX), typeof(double), typeof(Root), new PropertyMetadata(default(double)));
        public double ContentX
        {
            get { return (double) GetValue(ContentXProperty); }
            set { SetValue(ContentXProperty, value); }
        }

        public static readonly DependencyProperty ContentYProperty = DependencyProperty.Register(nameof(ContentY), typeof(double), typeof(Root), new PropertyMetadata(default(double)));
        public double ContentY
        {
            get { return (double) GetValue(ContentYProperty); }
            set { SetValue(ContentYProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Root), new PropertyMetadata(default(double)));
        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(Root), new PropertyMetadata(default(double)));

        public double LineThickness
        {
            get { return (double) GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        #endregion

        private const double Scale = 0.8;

        public Root()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            IndexTransform = new ScaleTransform(Scale, Scale);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var childSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Index?.Measure(childSize);
            (Content as UIElement)?.Measure(childSize);

            LineThickness = FontSize/10.0;

            var index = Index;
            var content = Content as UIElement;
            var indexHeight = index?.DesiredSize.Height ?? 0d * Scale;
            var indexWidth = index?.DesiredSize.Width ?? 0d * Scale;
            var contentHeight = content?.DesiredSize.Height ?? 0d;
            var contentWidth = content?.DesiredSize.Width ?? 0d;
            
            var rootWidth = FontSize/2.0;
            var width = indexWidth + contentWidth + rootWidth;

            var middleOffset = Math.Max(indexHeight, contentHeight/2.0) + 3.0;
            var height = middleOffset + contentHeight/2.0 + LineThickness *4.0;
            var contentTop = middleOffset - contentHeight/2.0 + 2.0*LineThickness;
            
            if(double.IsNaN(Width) || Math.Abs(Width - width) > double.Epsilon)
                Width = width;

            if(double.IsNaN(Height) || Math.Abs(Height - height) > double.Epsilon)
                Height = height;

            BaselineOffset = contentTop + content.GetBaselineOffset();
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var rootWidth = FontSize/2.0;
            var content = Content as UIElement;
            var contentHeight = content?.DesiredSize.Height ?? 0d;
            var contentWidth = content?.DesiredSize.Width ?? 0d;
            var indexHeight = (Index?.DesiredSize.Height ?? 0d)*Scale;
            var indexWidth = (Index?.DesiredSize.Width ?? 0d)*Scale;

            var middleOffset = Math.Max(indexHeight, contentHeight/2.0) + 3.0;
            var height = middleOffset + contentHeight/2.0;
            var width = indexWidth + contentWidth + rootWidth;
            var topLineY = middleOffset - contentHeight/2.0 + LineThickness/2.0;
            var contentLeft = indexWidth + rootWidth;

            ContentX = contentLeft;
            ContentY = topLineY + LineThickness;
            Points = new PointCollection
            {
                new Point(0, middleOffset), // left point
                new Point(indexWidth, middleOffset), // straight line
                new Point(indexWidth + rootWidth/2.0, height + LineThickness), // bottom point
                new Point(indexWidth + rootWidth, topLineY), // top point
                new Point(width, topLineY) // top right point
            };

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
