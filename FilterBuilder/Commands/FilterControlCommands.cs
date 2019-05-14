using System.Windows.Input;

namespace CmcScriptNet.FilterBuilder.Commands
{
    public static class FilterControlCommands
    {
        // TODO move to new class SingleValueControlCommands
        public static readonly RoutedCommand ShowFieldValueOptions = new RoutedCommand
        (
            nameof(ShowFieldValueOptions),
            typeof(FilterControlCommands),
            new InputGestureCollection() { }
        );

        public static readonly RoutedCommand Reset = new RoutedCommand
        (
            nameof(Reset),
            typeof(FilterControlCommands),
            new InputGestureCollection() { }
        );
    }
}
