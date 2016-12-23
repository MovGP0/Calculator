using System;
using System.Diagnostics;
using System.Windows;
using Calculator.Keypad;
using Serilog;

namespace Calculator.Main
{
    [TemplatePart(Name="PART_Keypad", Type=typeof(Keypad.Keypad))]
    public partial class MainPage
    {
        private static ILogger Log { get; } = Serilog.Log.ForContext<MainPage>();
        
        private KeypadViewModel KeypadViewModel { get; }
        
        public MainPage(KeypadViewModel keypadViewModel)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            Debug.Assert(keypadViewModel != null);
            KeypadViewModel = keypadViewModel;
        }
        
        public override void OnApplyTemplate()
        {
            var partKeypad = (Keypad.Keypad)Template.FindName("PART_Keypad", this);
            partKeypad.DataContext = KeypadViewModel;
            
            base.OnApplyTemplate();
        }
    }
}
