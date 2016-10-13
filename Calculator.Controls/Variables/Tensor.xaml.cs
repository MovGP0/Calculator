using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls.Variables
{
    public partial class Tensor
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Tensor), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(
            nameof(BaselineOffset), typeof(double), typeof(Tensor), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        static Tensor()
        {
            FontSizeProperty.OverrideMetadata(typeof(Tensor), 
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, OnMeasureInvalidated));
        }

        private static void OnMeasureInvalidated(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            InvalidateParents(sender);
        }

        private static void InvalidateParents(DependencyObject sender)
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(sender) as UIElement;
                if (parent == null) return;
                parent.InvalidateMeasure();
                sender = parent;
            }
        }

        public Tensor()
        {
            InitializeComponent();
        }
        
        protected override Size MeasureOverride(Size constraint)
        {
            var tb = GetTextblock();

            tb.Measure(constraint);
            BaselineOffset = tb.BaselineOffset;

            var width = Padding.Left + tb.DesiredSize.Width + Padding.Right;
            var height = Padding.Top + tb.DesiredSize.Height + Padding.Bottom;
            
            return new Size(width, height);
        }

        private TextBlock GetTextblock()
        {
            return new TextBlock
            {
                Text = Text,
                Margin = Margin, 
                FontSize = FontSize,
                Padding = Padding,
                FontFamily = FontFamily, 
                FontStretch = FontStretch,
                FontStyle = FontStyles.Italic, 
                FontWeight = FontWeights.Normal
            };
        }
        
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var tb = GetTextblock();

            tb.Measure(arrangeBounds);
            BaselineOffset = tb.BaselineOffset;

            base.ArrangeOverride(arrangeBounds);

            var width = Padding.Left + tb.DesiredSize.Width + Padding.Right;
            var height = Padding.Top + tb.DesiredSize.Height + Padding.Bottom;

            return new Size(width, height);
        }
    }
}
