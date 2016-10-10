using System.Windows.Input;
using Calculator.Controls;
using MaterialDesignThemes.Wpf;

namespace Calculator.Keypad
{
    public sealed class KeypadViewModel
    {
        public ICommand OpenTrigDialogCommand => new Command(OpenTrigDialog);

        private static async void OpenTrigDialog(object o)
        {
            var view = new TrigDialog
            {
                DataContext = new TrigDialogViewModel()
            };
            
            var result = await DialogHost.Show(view, "RootDialog");
        }
    }
}