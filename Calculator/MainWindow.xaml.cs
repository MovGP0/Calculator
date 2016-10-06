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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var adorner = new BaselineAdorner(RootElement);
            var adornerLayer = AdornerLayer.GetAdornerLayer(RootElement);
            adornerLayer.Add(adorner);
        }
    }
}
