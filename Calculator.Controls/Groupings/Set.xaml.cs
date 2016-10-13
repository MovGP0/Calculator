using System.Windows;
using System.Windows.Media;

namespace Calculator.Controls.Groupings
{
    public partial class Set
    {
        #region Dependent Properties
        public static readonly DependencyProperty ContentXProperty = DependencyProperty.Register(nameof(ContentX), typeof(double), typeof(Set), new PropertyMetadata(default(double)));

        public double ContentX
        {
            get { return (double) GetValue(ContentXProperty); }
            set { SetValue(ContentXProperty, value); }
        }

        public static readonly DependencyProperty ContentYProperty = DependencyProperty.Register(nameof(ContentY), typeof(double), typeof(Set), new PropertyMetadata(default(double)));

        public double ContentY
        {
            get { return (double) GetValue(ContentYProperty); }
            set { SetValue(ContentYProperty, value); }
        }

        public static readonly DependencyProperty PointsLeftProperty = DependencyProperty.Register(nameof(PointsLeft), typeof(PathGeometry), typeof(Set), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry PointsLeft
        {
            get { return (PathGeometry) GetValue(PointsLeftProperty); }
            set { SetValue(PointsLeftProperty, value); }
        }

        public static readonly DependencyProperty PointsRightProperty = DependencyProperty.Register(nameof(PointsRight), typeof(PathGeometry), typeof(Set), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry PointsRight
        {
            get { return (PathGeometry) GetValue(PointsRightProperty); }
            set { SetValue(PointsRightProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(Set), new PropertyMetadata(default(double)));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(Set), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }
        #endregion

        public Set()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var childSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var child = (Content as UIElement);
            child?.Measure(childSize);

            var childWidth = child?.DesiredSize.Width ?? 0d;
            var childHeight = child?.DesiredSize.Height ?? 0d;
            var childBaselineOffset = child.GetBaselineOffset();
            var lineWidth = FontSize/10d;

            var bracketWidth = FontSize/2d;
            
            var width = bracketWidth + childWidth + bracketWidth + lineWidth*2.0;
            var height = childHeight + lineWidth;

            StrokeThickness = lineWidth;
            BaselineOffset = childBaselineOffset + lineWidth/2.0;
            PointsLeft = GetLeftBracket(bracketWidth, childHeight, childBaselineOffset - FontSize/4.0, lineWidth);
            PointsRight = GetRightBracket(bracketWidth, childHeight, childBaselineOffset - FontSize/4.0, lineWidth, bracketWidth+childWidth+lineWidth*2.0);

            return new Size(width, height);
        }

        private static PathGeometry GetLeftBracket(double bracketWidth, double bracketHeight, double centerOffset, double lineWidth)
        {
            var yOffset = lineWidth/2.0;
            var arcRadius = bracketWidth/2d;
            var radi = new Size(arcRadius, arcRadius);
            var figure = new PathFigure(new Point(bracketWidth, yOffset), new PathSegment[]
            {
                new ArcSegment(new Point(bracketWidth-arcRadius, 0+arcRadius+yOffset), radi, 0.0, false, SweepDirection.Counterclockwise, true),
                new LineSegment(new Point(arcRadius, centerOffset-arcRadius+yOffset), true),
                new ArcSegment(new Point(0, centerOffset+yOffset), radi, 0.0, false, SweepDirection.Clockwise, true),
                new ArcSegment(new Point(arcRadius, centerOffset+arcRadius+yOffset), radi, 0.0, false, SweepDirection.Clockwise, true),
                new LineSegment(new Point(arcRadius, bracketHeight-arcRadius+yOffset), true), 
                new ArcSegment(new Point(bracketWidth, bracketHeight+yOffset), radi, 0.0, false, SweepDirection.Counterclockwise, true) 
            }, false);
            
            return new PathGeometry(new [] {figure});
        }

        private static PathGeometry GetRightBracket(double bracketWidth, double bracketHeight, double centerOffset,
            double lineWidth, double xOffset)
        {
            var yOffset = lineWidth/2.0;
            var arcRadius = bracketWidth/2d;
            var radi = new Size(arcRadius, arcRadius);
            var figure = new PathFigure(new Point(xOffset, yOffset), new PathSegment[]
            {
                new ArcSegment(new Point(xOffset+arcRadius, yOffset+arcRadius), radi, 0.0, false, SweepDirection.Clockwise, true),
                new LineSegment(new Point(arcRadius+xOffset, centerOffset-arcRadius+yOffset), true),
                new ArcSegment(new Point(bracketWidth+xOffset, centerOffset+yOffset), radi, 0.0, false, SweepDirection.Counterclockwise, true),
                new ArcSegment(new Point(xOffset+arcRadius, centerOffset+arcRadius+yOffset), radi, 0.0, false, SweepDirection.Counterclockwise, true),
                new LineSegment(new Point(xOffset+arcRadius, bracketHeight-arcRadius+yOffset), true), 
                new ArcSegment(new Point(xOffset, bracketHeight+yOffset), radi, 0.0, false, SweepDirection.Clockwise, true) 
            }, false);
            
            return new PathGeometry(new [] {figure});
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var child = (Content as UIElement);
            var childWidth = child?.DesiredSize.Width ?? 0d;
            var childHeight = child?.DesiredSize.Height ?? 0d;
            var lineWidth = FontSize/10d;
            var bracketWidth = FontSize/1.5d;
            
            var width = bracketWidth + childWidth + bracketWidth + lineWidth*2.0;
            var height = childHeight + lineWidth;

            ContentX = bracketWidth;
            ContentY = lineWidth/2.0;

            base.ArrangeOverride(arrangeBounds);
            return new Size(width, height);
        }
    }
}
