using Windows.Foundation;
using Windows.UI.Xaml;

namespace Calculator.Components
{
    public sealed partial class Fraction
    {
        private static readonly DependencyProperty NumeratorProperty = DependencyProperty.Register(nameof(Numerator), typeof(object), typeof(Fraction), new PropertyMetadata(null));
        public object Numerator
        {
            get { return (object)GetValue(NumeratorProperty); }
            set { SetValue(NumeratorProperty, value); }
        }

        private static readonly DependencyProperty DenominatorProperty = DependencyProperty.Register(nameof(Denominator), typeof(object), typeof(Fraction), new PropertyMetadata(null));
        public object Denominator
        {
            get { return (object)GetValue(DenominatorProperty); }
            set { SetValue(DenominatorProperty, value); }
        }
        
        private static readonly DependencyProperty BaselineProperty = DependencyProperty.Register(nameof(Baseline), typeof(double), typeof(Fraction), new PropertyMetadata(0d));
        public double Baseline
        {
            get { return (double)GetValue(BaselineProperty); }
            set { SetValue(BaselineProperty, value); }
        }

        public Fraction()
        {
            InitializeComponent();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(0d, 0d);
        }
    }
}
