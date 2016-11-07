using System.Diagnostics;
using Calculator.Keypad;
using System.Windows;

namespace Calculator.Pages
{
    [TemplatePart(Name="PART_Keypad", Type=typeof(Keypad.Keypad))]
    public partial class MainFrame
    {
        private Keypad.Keypad PartKeypad { get; set; }
        private KeypadViewModel KeypadViewModel { get; }

        public MainFrame(KeypadViewModel keypadViewModel)
        {
            InitializeComponent();

            Debug.Assert(keypadViewModel != null);
            KeypadViewModel = keypadViewModel;
        }

        public override void OnApplyTemplate()
        {
            var partKeypad = (Keypad.Keypad)Template.FindName("PART_Keypad", this);
            partKeypad.DataContext = KeypadViewModel;
            PartKeypad = partKeypad;

            base.OnApplyTemplate();
        }
    }
}
