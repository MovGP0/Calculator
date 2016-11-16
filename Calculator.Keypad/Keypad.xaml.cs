using System;
using Serilog;

namespace Calculator.Keypad
{
    public partial class Keypad
    {
        public Keypad()
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
        }
    }
}
