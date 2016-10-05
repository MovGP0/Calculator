using System.Windows;
using System.Windows.Controls;

namespace Calculator.Controls
{
    public static class UiElementExtensions
    {
        public static double GetBaselineOffset(this UIElement control)
        {
            if (control == null)
                return 0d;

            // handle known types
            var textBlock = control as TextBlock;
            if (textBlock != null)
            {
                return textBlock.BaselineOffset;
            }

            if (control is TextBox) return 2d;
            if (control is ComboBox) return 2d;
            
            // use reflection
            var baselinePropertyInfo = control.GetType().GetProperty("BaselineOffset", typeof(double));
            if (baselinePropertyInfo != null)
            {
                return (double)baselinePropertyInfo.GetValue(control, null);
            }
            
            // use height as fallback
            return control.DesiredSize.Height;
        }
    }
}
