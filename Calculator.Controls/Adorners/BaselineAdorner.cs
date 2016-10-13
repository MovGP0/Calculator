using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Calculator.Controls.Adorners
{
    public sealed class BaselineAdorner : Adorner
    {
        public BaselineAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var width = AdornedElement.DesiredSize.Width;
            var baselineOffset = AdornedElement.GetBaselineOffset();
            var pen = new Pen(Brushes.Aqua, 1.0);
            drawingContext.DrawLine(pen, new Point(0, baselineOffset), new Point(width, baselineOffset));
        }
    }
}