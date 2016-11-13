using System;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Calculator.Pages
{
    public sealed class NavigateToTrainCommand : ICommand
    {
        private NavigationService _navigationService;
        public NavigationService NavigationService
        {
            private get
            {
                return _navigationService;
            }
            set
            {
                if(ReferenceEquals(_navigationService, value)) return;

                _navigationService = value;
                OnCanExecuteChanged();
            }
        }

        private Func<GestureTrainingPage> GestureTrainingFrameFactory { get; }

        public NavigateToTrainCommand(Func<GestureTrainingPage> gestureTrainingFrameFactory)
        {
            GestureTrainingFrameFactory = gestureTrainingFrameFactory;
        }
        
        public bool CanExecute(object parameter)
        {
            return _navigationService != null;
        }

        public void Execute(object parameter)
        {
            NavigationService.Navigate(GestureTrainingFrameFactory());
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