using System;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Calculator.Pages
{
    public sealed class NavigateToMainCommand : ICommand
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
                if (ReferenceEquals(_navigationService, value)) return;

                _navigationService = value;
                _navigationService.Navigated += NavigationServiceOnNavigated;

                OnCanExecuteChanged();
            }
        }

        private void NavigationServiceOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            OnCanExecuteChanged();
        }

        private Func<MainPage> MainPageFactory { get; }

        public NavigateToMainCommand(Func<MainPage> mainPageFactory)
        {
            MainPageFactory = mainPageFactory;
        }

        public bool CanExecute(object parameter)
        {
            return NavigationService != null
                   && NavigationService.Content.GetType() != typeof(MainPage);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(null)) return;

            NavigationService.Navigate(MainPageFactory());
        }

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}