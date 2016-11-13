using System.Diagnostics;
using Calculator.Keypad;
using System.Windows;

namespace Calculator.Pages
{
    [TemplatePart(Name="PART_Keypad", Type=typeof(Keypad.Keypad))]
    public partial class MainPage
    {
        private KeypadViewModel KeypadViewModel { get; }
        public NavigateToTrainCommand NavigateToTrainCommand { get; private set; }

        public MainPage(KeypadViewModel keypadViewModel, NavigateToTrainCommand navigateToTrainCommand)
        {
            InitializeComponent();

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
