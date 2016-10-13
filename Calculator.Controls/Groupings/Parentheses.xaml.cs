using System.Windows;
using System.Windows.Media;

namespace Calculator.Controls.Groupings
{
    public partial class Parentheses
    {
        #region Dependent Properties

        public static readonly DependencyProperty CanvasWidthProperty = DependencyProperty.Register(nameof(CanvasWidth), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double CanvasWidth
        {
            get { return (double) GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public static readonly DependencyProperty CanvasHeightProperty = DependencyProperty.Register(nameof(CanvasHeight), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double CanvasHeight
        {
            get { return (double) GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        public static readonly DependencyProperty RightBracketXProperty = DependencyProperty.Register(nameof(RightBracketX), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double RightBracketX
        {
            get { return (double) GetValue(RightBracketXProperty); }
            set { SetValue(RightBracketXProperty, value); }
        }

        public static readonly DependencyProperty RightBracketProperty = DependencyProperty.Register(nameof(RightBracket), typeof(PathGeometry), typeof(Parentheses), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry RightBracket
        {
            get { return (PathGeometry) GetValue(RightBracketProperty); }
            set { SetValue(RightBracketProperty, value); }
        }

        public static readonly DependencyProperty LeftBracketProperty = DependencyProperty.Register(nameof(LeftBracket), typeof(PathGeometry), typeof(Parentheses), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry LeftBracket
        {
            get { return (PathGeometry) GetValue(LeftBracketProperty); }
            set { SetValue(LeftBracketProperty, value); }
        }

        public static readonly DependencyProperty ContentXProperty = DependencyProperty.Register(nameof(ContentX), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double ContentX
        {
            get { return (double) GetValue(ContentXProperty); }
            set { SetValue(ContentXProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Parentheses), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }
        #endregion

        public Parentheses()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var childSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var child = Content as UIElement;
            child?.Measure(childSize);

            var childWidth = child?.DesiredSize.Width ?? 0d;
            var childHeight = child?.DesiredSize.Height ?? 0d;
            var childBaselineOffset = child.GetBaselineOffset();
            var lineWidth = FontSize/10d;
            var bracketWidth = FontSize/2.0d;

            var canvasWidth = bracketWidth + lineWidth + childWidth + lineWidth + bracketWidth;
            var canvasHeight = lineWidth + childHeight + lineWidth;

            var width = Padding.Left + canvasWidth + Padding.Right;
            var height = Padding.Left + canvasHeight + Padding.Right;

            StrokeThickness = lineWidth;
            BaselineOffset = childBaselineOffset + lineWidth/2.0;
            LeftBracket = GetLeftBracket(bracketWidth, childHeight, lineWidth);
            RightBracket = GetRightBracket(bracketWidth, childHeight, lineWidth);
            RightBracketX = bracketWidth + lineWidth + childWidth + lineWidth;
            ContentX = bracketWidth + lineWidth;
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            return new Size(width, height);
        }

        private static PathGeometry GetLeftBracket(double bracketWidth, double bracketHeight, double lineWidth)
        {
            var yOffset = lineWidth/2.0;
            var figure = new PathFigure(new Point(bracketWidth, yOffset), new PathSegment[]
            {
                new ArcSegment(new Point(bracketWidth, bracketHeight-yOffset), new Size(bracketWidth*4, bracketHeight-lineWidth), 0, false, SweepDirection.Counterclockwise, true), 
            }, false);
            
            return new PathGeometry(new [] {figure});
        }

        private static PathGeometry GetRightBracket(double bracketWidth, double bracketHeight, double lineWidth)
        {
            var yOffset = lineWidth/2.0;
            var figure = new PathFigure(new Point(0, yOffset), new PathSegment[]
            {
                new ArcSegment(new Point(0, bracketHeight-yOffset), new Size(bracketWidth*4, bracketHeight-lineWidth), 0, false, SweepDirection.Clockwise, true), 
            }, false);
            
            return new PathGeometry(new [] {figure});
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var child = Content as UIElement;
            
            var childWidth = child?.DesiredSize.Width ?? 0d;
            var childHeight = child?.DesiredSize.Height ?? 0d;
            var lineWidth = FontSize/10d;
            var bracketWidth = FontSize/2.0d;
            
            var width = Padding.Left + bracketWidth + lineWidth + childWidth + lineWidth + bracketWidth + Padding.Right;
            var height = Padding.Top + lineWidth + childHeight + lineWidth + Padding.Bottom;

            base.ArrangeOverride(arrangeBounds);

            return new Size(width, height);
        }
    }
}
