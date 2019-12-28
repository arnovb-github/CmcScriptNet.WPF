using FilterBuilder.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    public class FilterToStringConverter : IValueConverter
    {
        // not too keen on this workaround
        // the parameter is passed in using x:Reference in the xaml
        // it represents a combobox (for now). This is an unwanted and fragile coupling
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is ICursorFilter)) { return null; }
            if (parameter == null) { return value.ToString(); }
            var cb = (System.Windows.Controls.ComboBox)parameter;
            return FilterStringCreator.ToString((ICursorFilter)value, (FilterOutputFormat)cb.SelectedValue);
        }

        //// working version but no custom output
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (!(value is ICursorFilter)) { return null; }
        //    return value.ToString();
        //}

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
