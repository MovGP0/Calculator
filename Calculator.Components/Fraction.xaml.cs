using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Calculator.Components
{
    [ContentProperty(Name = nameof(Numerator))]
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
            DataContext = this;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var numerator = Numerator as UIElement;
            var denominator = Denominator as UIElement;

            numerator?.Measure(availableSize);
            denominator?.Measure(availableSize);

            var height = (numerator?.DesiredSize.Height ?? 0d) + (denominator?.DesiredSize.Height ?? 0d) + 3d;
            var width = Math.Max(numerator?.DesiredSize.Width ?? 0d, denominator?.DesiredSize.Width ?? 0d);

            return new Size(Math.Min(width, availableSize.Width), Math.Min(height, availableSize.Height));
        }
        
        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    return finalSize;
        //}
    }
}
