using System.Windows;
using Calculator.DependencyInjection;
using DryIoc;

namespace Calculator
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var locator = ResourceLocator.Get();
            var mainWindow = locator.Resolve<Pages.MainWindow>();
            mainWindow.Show();
        }
    }
}
