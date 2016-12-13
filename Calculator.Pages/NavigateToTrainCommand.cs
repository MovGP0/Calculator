using System;
using System.Windows.Input;
using System.Windows.Navigation;
using Serilog;

namespace Calculator.Pages
{
    public sealed class NavigateToTrainCommand : ICommand
    {
        private static ILogger Log => Serilog.Log.Logger.ForContext<NavigateToTrainCommand>();

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
                _navigationService.Navigated += NavigationServiceOnNavigated;

                OnCanExecuteChanged();
            }
        }

        private void NavigationServiceOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            OnCanExecuteChanged();
        }

        private Func<GestureTrainingPage> GestureTrainingPageFactory { get; }

        public NavigateToTrainCommand(Func<GestureTrainingPage> gestureTrainingPageFactory)
        {
            GestureTrainingPageFactory = gestureTrainingPageFactory;
        }
        
        public bool CanExecute(object parameter)
        {
            return NavigationService != null 
                && _navigationService.Content.GetType() != typeof(GestureTrainingPage);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(null)) return;
            
            Log.Information("Navigating to TrainingPage");
            NavigationService.Navigate(GestureTrainingPageFactory());
        }

        public event EventHandler CanExecuteChanged;
        
        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}