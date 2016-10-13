using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Calculator.Controls.Adorners
{
    /// <summary>
    /// Adds a flashing cursor to an given UI element.
    /// </summary>
    /// <example>
    /// <code>
    /// var adorner = new SelectionAdorner(SomeUIElement);
    /// var adornerLayer = AdornerLayer.GetAdornerLayer(SomeUIElement);
    /// adornerLayer.Add(adorner);
    /// adornerLayer.Remove(adorner);
    /// </code>
    /// </example>
    public sealed class SelectionAdorner : Adorner
    {
        private readonly Color _color = Colors.Crimson;
        private readonly double _brushWidth = 2.0;

        public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            Loaded += OnLoaded;
        }

        public SelectionAdorner(UIElement adornedElement, Color color, double brushWidth) : this(adornedElement)
        {
            _color = color;
            _brushWidth = brushWidth;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(storyboard, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));

            storyboard.Begin(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var brush = new SolidColorBrush(_color);
            var pen = new Pen(brush, _brushWidth);
            var rect = new Rect(AdornedElement.DesiredSize);

            drawingContext.DrawLine(pen, rect.BottomLeft, rect.BottomRight);
        }
    }
}
