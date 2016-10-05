using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.Controls
{
    public sealed class AlignPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var children = Children;

            var stackDesiredSize = new Size();
            var childSize = new Size(double.PositiveInfinity, availableSize.Height);

            foreach (var child in children.OfType<UIElement>().Where(c => c != null))
            {
                child.Measure(childSize);
                var offset = child.GetBaselineOffset();

                stackDesiredSize.Width += child.DesiredSize.Width;
                stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, child.DesiredSize.Height + offset);
            }

            return stackDesiredSize; 
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var children = Children;
            var rcChild = new Rect(new Point(0d, 0d), arrangeSize);
            var previousChildSize = 0d;

            var maximumOffset = children.OfType<UIElement>().Max(c => c.GetBaselineOffset());

            foreach (var child in children.OfType<UIElement>().Where(c => c != null))
            {
                var offset = child.GetBaselineOffset();
                
                rcChild.X += previousChildSize;
                rcChild.Y = maximumOffset - offset;

                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = child.DesiredSize.Width;

                rcChild.Height = Math.Max(arrangeSize.Height - offset, child.DesiredSize.Height);

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }
    }
}
