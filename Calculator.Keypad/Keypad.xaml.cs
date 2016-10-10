namespace Calculator.Keypad
{
    public partial class Keypad
    {
        public Keypad()
        {
            InitializeComponent();
            DataContext = new KeypadViewModel();
        }
    }
}
