using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Calculator.Controls.Adorners
{
    /// <summary>
    /// Highlights an selected UI element.
    /// </summary>
    /// <example>
    /// <code>
    /// var adorner = new HighlightAdorner(SomeUIElement);
    /// var adornerLayer = AdornerLayer.GetAdornerLayer(SomeUIElement);
    /// adornerLayer?.Add(adorner);
    /// adornerLayer?.Remove(adorner);
    /// </code>
    /// </example>
    /// <remarks>
    /// Make sure that the template of the control has an &lt;AdornerDecorator&gt; where you want to draw the adorner.
    /// </remarks>
    public sealed class HighlightAdorner : Adorner
    {
        private readonly Color _color = Colors.Yellow;
        private readonly double _opacity = 0.8;

        public HighlightAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        public HighlightAdorner(UIElement adornedElement, Color color, double opacity) : base(adornedElement)
        {
            _color = color;
            _opacity = opacity;
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush(_color) {Opacity = _opacity};
            var pen = new Pen(Brushes.Transparent, 0);
            var rect = new Rect(AdornedElement.DesiredSize);

            drawingContext.DrawRectangle(brush, pen, rect);
        }
    }
}