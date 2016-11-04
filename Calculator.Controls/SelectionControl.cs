using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Calculator.Controls
{
    [TemplatePart(Name="PART_Canvas", Type=typeof(Canvas))]
    [TemplatePart(Name="PART_SelectionRectangle", Type=typeof(Rectangle))]
    public sealed class SelectionControl : UserControl
    {
        private bool IsMouseDown { get; set; }
        private Point MouseDownPosition { get; set; }
        private Canvas PartCanvas { get; set; }
        private Rectangle PartSelectionRectangle { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartCanvas = GetPartCanvas();
            PartSelectionRectangle = GetPartSelectionRectangle();
        }

        private Rectangle GetPartSelectionRectangle()
        {
            var partSelectionRectangle = Template.FindName("PART_SelectionRectangle", this) as Rectangle;
            if (partSelectionRectangle == null)
                throw new InvalidOperationException("Could not find 'PART_SelectionRectangle' in control template.");

            partSelectionRectangle.Visibility = Visibility.Collapsed;

            return partSelectionRectangle;
        }

        private Canvas GetPartCanvas()
        {
            var partCanvas = Template.FindName("PART_Canvas", this) as Canvas;
            if (partCanvas == null)
                throw new InvalidOperationException("Could not find 'PART_Canvas' in control template.");

            partCanvas.MouseDown += OnMouseDown;
            partCanvas.MouseUp += OnMouseUp;
            partCanvas.MouseMove += OnMouseMove;
            return partCanvas;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            MouseDownPosition = e.GetPosition(PartCanvas);
            PartCanvas.CaptureMouse();
       
            Canvas.SetLeft(PartSelectionRectangle, MouseDownPosition.X);
            Canvas.SetTop(PartSelectionRectangle, MouseDownPosition.Y);
            PartSelectionRectangle.Width = 0;
            PartSelectionRectangle.Height = 0;
            
            PartSelectionRectangle.Visibility = Visibility.Visible;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
            PartCanvas.ReleaseMouseCapture();
            
            PartSelectionRectangle.Visibility = Visibility.Collapsed;

            var mouseUpPos = e.GetPosition(PartCanvas);

            // TODO: 
            //
            // The mouse has been released, check to see if any of the items 
            // in the other canvas are contained within mouseDownPos and 
            // mouseUpPos, for any that are, select them!
            //
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMouseDown) return;
            
            var mousePos = e.GetPosition(PartCanvas);

            if (MouseDownPosition.X < mousePos.X)
            {
                Canvas.SetLeft(PartSelectionRectangle, MouseDownPosition.X);
                PartSelectionRectangle.Width = mousePos.X - MouseDownPosition.X;
            }
            else
            {
                Canvas.SetLeft(PartSelectionRectangle, mousePos.X);
                PartSelectionRectangle.Width = MouseDownPosition.X - mousePos.X;
            }

            if (MouseDownPosition.Y < mousePos.Y)
            {
                Canvas.SetTop(PartSelectionRectangle, MouseDownPosition.Y);
                PartSelectionRectangle.Height = mousePos.Y - MouseDownPosition.Y;
            }
            else
            {
                Canvas.SetTop(PartSelectionRectangle, mousePos.Y);
                PartSelectionRectangle.Height = MouseDownPosition.Y - mousePos.Y;
            }
        }
    }
}