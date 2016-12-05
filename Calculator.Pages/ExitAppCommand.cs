using System;
using System.Windows;
using System.Windows.Input;

namespace Calculator.Pages
{
    public sealed class ExitAppCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        private event EventHandler CanExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CanExecuteChanged += value; }
            remove { CanExecuteChanged -= value; }
        }

        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}