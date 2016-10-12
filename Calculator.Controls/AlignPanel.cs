using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls
{
    public sealed class AlignPanel : Panel
    {
        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(nameof(BaselineOffset), typeof(double), typeof(AlignPanel), new PropertyMetadata(default(double)));
        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var childSize = new Size(double.PositiveInfinity, availableSize.Height);
            var children = Children.OfType<UIElement>().Where(c => c != null).ToArray();
            
            foreach (var child in children)
            {
                child.Measure(childSize);
            }

            var maximumOffset = children.Max(c => c.GetBaselineOffset());
            
            var height = children.Select(c => new {
                c.DesiredSize.Height,
                Offset = c.GetBaselineOffset()
            }).Max(c => c.Height + maximumOffset - c.Offset);

            var width = children.Sum(c => c.DesiredSize.Width);

            BaselineOffset = maximumOffset;

            return new Size(width, height); 
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var children = Children.OfType<UIElement>().Where(c => c != null).ToArray();
            var rcChild = new Rect(new Point(0d, 0d), arrangeSize);
            var previousChildSize = 0d;

            var maximumOffset = children.Max(c => c.GetBaselineOffset());

            foreach (var child in children)
            {
                var offset = child.GetBaselineOffset();
                
                rcChild.X += previousChildSize;
                rcChild.Y = maximumOffset-offset;

                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = child.DesiredSize.Width;
                rcChild.Height = child.DesiredSize.Height;

                child.Arrange(new Rect(rcChild.Location, rcChild.Size));
            }

            var height = children.Select(c => new {
                c.DesiredSize.Height,
                Offset = c.GetBaselineOffset()
            }).Max(c => c.Height + maximumOffset - c.Offset);

            var width = children.Sum(c => c.DesiredSize.Width);

            return new Size(width, height);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawLine(new Pen(new SolidColorBrush(Colors.Orange){ Opacity = 0.2 }, 2.4), new Point(0,BaselineOffset), new Point(ActualWidth,BaselineOffset));
        }
    }
}
