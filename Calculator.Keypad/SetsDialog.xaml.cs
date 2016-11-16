using System;
using Serilog;

namespace Calculator.Keypad
{
    public partial class SetsDialog
    {
        public SetsDialog()
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
