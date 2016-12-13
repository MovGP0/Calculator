using System.Windows.Input;

namespace Calculator.Common
{
    public static class RoutedCommands
    {
        public static RoutedCommand NavigateToMain { get; } = new RoutedCommand();
        public static RoutedCommand NavigateToTrain { get; } = new RoutedCommand();
    }
}
