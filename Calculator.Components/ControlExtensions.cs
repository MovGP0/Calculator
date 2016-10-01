using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace Calculator.Components
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Get the Baseline of the contained object if it supports it.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static double GetBaseline(this DependencyObject control)
        {
            const double defaultValue = 0d;
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            if(control == null) return defaultValue;

            var type = control.GetType();
            
            var baselineProperty = type.GetFields(flags)
                .Where(prop => prop.FieldType == typeof(double))
                .SingleOrDefault(prop => prop.Name == "Baseline");

            if (baselineProperty == null)
            {
                var uiElement = control as UIElement;
                return uiElement?.DesiredSize.Height ?? defaultValue;
            }

            var baselinePropertyInfo = type.GetProperty("Baseline", null);
            return (double)baselinePropertyInfo.GetValue(type, null);
        }
    }
}
