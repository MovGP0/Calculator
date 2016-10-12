using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Calculator.Controls
{
    public partial class Brackets
    {
        private static void InvalidateMeasure(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            (source as UIElement)?.InvalidateMeasure();
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(UIElement), typeof(Brackets), new UIPropertyMetadata(default(UIElement), InvalidateMeasure));
        public UIElement Left
        {
            get { return (UIElement) GetValue(LeftProperty); }
            set { SetValue(LeftProperty,value); }
        }
        
        public static readonly DependencyProperty RightProperty = DependencyProperty.Register(nameof(Right), typeof(UIElement), typeof(Brackets), new UIPropertyMetadata(default(UIElement), InvalidateMeasure));
        public UIElement Right
        {
            get { return (UIElement) GetValue(RightProperty); }
            set { SetValue(RightProperty,value); }
        }
        
        public static readonly DependencyProperty LeftXProperty = DependencyProperty.Register(nameof(LeftX), typeof(double), typeof(Brackets), new UIPropertyMetadata(default(double)));
        public double LeftX
        {
            get { return (double) GetValue(LeftXProperty); }
            set { SetValue(LeftXProperty,value); }
        }

        public static readonly DependencyProperty ContentXProperty = DependencyProperty.Register(nameof(ContentX), typeof(double), typeof(Brackets), new UIPropertyMetadata(default(double)));
        public double ContentX
        {
            get { return (double) GetValue(ContentXProperty); }
            set { SetValue(ContentXProperty,value); }
        }

        public static readonly DependencyProperty RightXProperty = DependencyProperty.Register(nameof(RightX), typeof(double), typeof(Brackets), new UIPropertyMetadata(default(double)));
        public double RightX
        {
            get { return (double) GetValue(RightXProperty); }
            set { SetValue(RightXProperty,value); }
        }

        public static readonly DependencyProperty LeftTransformProperty = DependencyProperty.Register(nameof(LeftTransform), typeof(Transform), typeof(Brackets), new UIPropertyMetadata(default(Transform)));
        public Transform LeftTransform
        {
            get { return (Transform) GetValue(LeftTransformProperty); }
            set { SetValue(LeftTransformProperty,value); }
        }

        public static readonly DependencyProperty RightTransformProperty = DependencyProperty.Register(nameof(RightTransform), typeof(Transform), typeof(Brackets), new UIPropertyMetadata(default(Transform)));
        public Transform RightTransform
        {
            get { return (Transform) GetValue(RightTransformProperty); }
            set { SetValue(RightTransformProperty,value); }
        }

        private static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Brackets), new PropertyMetadata(default(double)));
        public double BaselineOffset
        {
            get { return (double)GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        public Brackets()
        {
            InitializeComponent();
        }

        private double _scaleFactor;
        private const double HorizontalScaleFactor = 1d;
        protected override Size MeasureOverride(Size constraint)
        {
            Left?.Measure(constraint);
            (Content as UIElement).Measure(constraint);
            Right?.Measure(constraint);

            var content = Content as UIElement;

            var leftDesiredHeight = Left?.DesiredSize.Height ?? 0d;
            var leftDesiredWidth = Left?.DesiredSize.Width ?? 0d;

            var contentDesiredHeight = content?.DesiredSize.Height ?? 0d;
            var contentDesiredWidth = content?.DesiredSize.Width ?? 0d;

            var rightDesiredHeight = Right?.DesiredSize.Height ?? 0d;
            var rightDesiredWidth = Right?.DesiredSize.Width ?? 0d;

            _scaleFactor = CalculateScaleFactor(leftDesiredHeight, rightDesiredHeight, contentDesiredHeight);

            var transform = new ScaleTransform
            {
                ScaleX = _scaleFactor * HorizontalScaleFactor,
                ScaleY = _scaleFactor,
                CenterX = 0,
                CenterY = 0
            };

            LeftTransform = transform;
            RightTransform = transform;
            
            var width = contentDesiredWidth
                + leftDesiredWidth * _scaleFactor * HorizontalScaleFactor 
                + rightDesiredWidth * _scaleFactor * HorizontalScaleFactor;

            var height = new []
            {
                leftDesiredHeight * _scaleFactor,
                contentDesiredHeight,
                rightDesiredHeight * _scaleFactor
            }.Max();
            
            BaselineOffset = content.GetBaselineOffset();

            return new Size(width, height);
        }

        private double CalculateScaleFactor(double leftDesiredHeight, double rightDesiredHeight, double childDesiredHeight)
        {
            var bracketHeight = GetBracketHeight(leftDesiredHeight, rightDesiredHeight);
            var scaleFactor = childDesiredHeight/bracketHeight;
            return scaleFactor;
        }

        private double GetBracketHeight(double leftDesiredHeight, double rightDesiredHeight)
        {
            if (leftDesiredHeight <= 0d && rightDesiredHeight <= 0d)
            {
                return FontSize;
            }

            return Math.Max(leftDesiredHeight, rightDesiredHeight);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var leftDesiredWidth = Left?.DesiredSize.Width ?? 0d;
            var contentDesiredWidth = (Content as UIElement)?.DesiredSize.Width ?? 0d;
            var rightDesiredWidth = Right?.DesiredSize.Width ?? 0d;

            LeftX = 0;
            ContentX = leftDesiredWidth * _scaleFactor * HorizontalScaleFactor;
            RightX = rightDesiredWidth * _scaleFactor * HorizontalScaleFactor + contentDesiredWidth;
            
            return base.ArrangeOverride(arrangeBounds); 
        }
    }
}
