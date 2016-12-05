using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Serilog;

namespace Calculator.Pages
{
    [TemplatePart(Name=PartMainFrameName, Type=typeof(Frame))]
    public partial class ShellWindow
    {
        internal const string PartMainFrameName = "PART_MainFrame";
        private Frame PartMainFrame { get; set; }
        private Func<MainPage> MainFrameFactory { get; }
        
        private NavigateToTrainCommand NavigateToTrainCommand { get; }
        private ExitAppCommand ExitAppCommand { get; }

        public ShellWindow(Func<MainPage> mainFrameFactory, ExitAppCommand exitAppCommand, NavigateToTrainCommand navigateToTrainCommand)
        {
            MainFrameFactory = mainFrameFactory;

            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }

            Loaded += OnLoaded;
            
            ExitAppCommand = exitAppCommand;
            NavigateToTrainCommand = navigateToTrainCommand;

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));

            InputBindings.Add(new InputBinding(ExitAppCommand, new KeyGesture(Key.X, ModifierKeys.Control)));
            InputBindings.Add(new InputBinding(NavigateToTrainCommand, new KeyGesture(Key.T, ModifierKeys.Control)));
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartMainFrame = GetPartMainFrame();
        }

        private Frame GetPartMainFrame()
        {
            var partMainFrame = (Frame) Template.FindName(PartMainFrameName, this);
            if (partMainFrame == null)
                throw new InvalidOperationException($"Cold not find '{PartMainFrameName}'.");

            return partMainFrame;
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            var navigationService = PartMainFrame.NavigationService;
            NavigateToTrainCommand.NavigationService = navigationService;
            navigationService.Navigate(MainFrameFactory());
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
    }
}
