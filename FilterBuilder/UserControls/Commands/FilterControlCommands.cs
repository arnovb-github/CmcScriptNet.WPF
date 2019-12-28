using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CmcScriptNet.FilterBuilder.UserControls.Commands
{
    public static class FilterControlCommands
    {
        public static readonly RoutedCommand ShowFieldValueOptions = new RoutedCommand
        (
            nameof(ShowFieldValueOptions),
            typeof(FilterControlCommands),
            new InputGestureCollection() { }
        );
    }
}
