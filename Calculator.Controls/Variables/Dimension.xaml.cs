using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator.Controls.Variables
{
    public partial class Dimension
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Dimension), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(
            nameof(BaselineOffset), typeof(double), typeof(Dimension), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        static Dimension()
        {
            FontSizeProperty.OverrideMetadata(typeof(Dimension), 
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

        public Dimension()
        {
            InitializeComponent();
        }

        private double _width;
        private double _height;

        protected override Size MeasureOverride(Size constraint)
        {
            var tb = GetTextblock();

            tb.Measure(constraint);
            BaselineOffset = tb.BaselineOffset;

            _width = tb.DesiredSize.Width;
            _height = tb.DesiredSize.Height;
            
            return new Size(_width, _height);
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
            
            return new Size(_width, _height);
        }
    }
}
