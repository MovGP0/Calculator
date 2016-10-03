using System;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Calculator.Components
{
    public sealed class AlignPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var stackDesiredSize = new Size();

            var children = Children;
            availableSize.Width = double.PositiveInfinity;
            
            foreach (var child in children.Where(c => c != null))
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                var offset = GetStackElementOffset(child);

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

            var maximumOffset = children.Max(c => GetStackElementOffset(c));

            foreach (var child in children.Where(c => c != null))
            {
                var offset = GetStackElementOffset(child);
                
                rcChild.X += previousChildSize;
                rcChild.Y = maximumOffset - offset;

                previousChildSize = child.DesiredSize.Width;
                rcChild.Width = child.DesiredSize.Width;

                rcChild.Height = Math.Max(arrangeSize.Height - offset, child.DesiredSize.Height);

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }

        private static double GetStackElementOffset(UIElement stackElement)
        {
            if (stackElement is TextBlock)
                return 5;
        
            if (stackElement is TextBox)
                return 2;

            if (stackElement is ComboBox)
                return 2;

            return GetBaselineOffset(stackElement);
        }

        public static double GetBaselineOffset(DependencyObject control)
        {
            const double defaultValue = 0d;
            if(control == null) return defaultValue;

            var type = control.GetType();
            
            var baselinePropertyInfo = type.GetProperty("BaselineOffset", typeof(double));
            if (baselinePropertyInfo != null)
            {
                return (double)baselinePropertyInfo.GetValue(control, null);
            }

            var uiElement = control as UIElement;
            return uiElement?.DesiredSize.Height ?? defaultValue;
        }
    }
}
