using System.Windows.Input;

namespace Calculator.Common
{
    public static class RoutedCommands
    {
        public static RoutedCommand NavigateToMain { get; } = new RoutedCommand("Navigate to main", typeof(RoutedCommands));
        public static RoutedCommand NavigateToTrain { get; } = new RoutedCommand("Navigate to train", typeof(RoutedCommands));
    }
}
