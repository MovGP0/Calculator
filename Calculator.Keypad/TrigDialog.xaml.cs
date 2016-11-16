using System;
using Serilog;

namespace Calculator.Keypad
{
    /// <summary>
    /// Interaction logic for TrigDialog.xaml
    /// </summary>
    public partial class TrigDialog
    {
        public TrigDialog()
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
