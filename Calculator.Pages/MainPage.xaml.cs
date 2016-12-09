using System;
using System.Diagnostics;
using Calculator.Keypad;
using System.Windows;
using Serilog;

namespace Calculator.Pages
{
    [TemplatePart(Name="PART_Keypad", Type=typeof(Keypad.Keypad))]
    public partial class MainPage
    {
        private static ILogger Log { get; } = Serilog.Log.ForContext<MainPage>();
        
        private KeypadViewModel KeypadViewModel { get; }
        public NavigateToTrainCommand NavigateToTrainCommand { get; }
        
        public MainPage(KeypadViewModel keypadViewModel, NavigateToTrainCommand navigateToTrainCommand)
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

            Debug.Assert(keypadViewModel != null);
            KeypadViewModel = keypadViewModel;

            Loaded += OnLoaded;
            NavigateToTrainCommand = navigateToTrainCommand;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigateToTrainCommand.NavigationService = NavigationService;
        }
        
        public override void OnApplyTemplate()
        {
            var partKeypad = (Keypad.Keypad)Template.FindName("PART_Keypad", this);
            partKeypad.DataContext = KeypadViewModel;
            
            base.OnApplyTemplate();
        }
    }
}
