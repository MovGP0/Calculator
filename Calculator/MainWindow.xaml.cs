using System.Windows;
using System.Windows.Documents;
using Calculator.Controls;

namespace Calculator
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            var rootAdorner = new BaselineAdorner(RootElement);
            var fractionAdorner = new BaselineAdorner(Fraction);

            //AdornerLayer.GetAdornerLayer(RootElement).Add(rootAdorner);
            //AdornerLayer.GetAdornerLayer(Fraction).Add(fractionAdorner);
        }
    }
}
