using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    [ValueConversion(typeof(CommenceFieldType), typeof(Visibility))]
    public abstract class BaseFieldTypeToVisibilityConverter : IValueConverter
    {
        public BaseFieldTypeToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is CommenceFieldType)) { return null; }
            return this.FieldTypes.Any(a => a.Equals(value)) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }

        public abstract IEnumerable<CommenceFieldType> FieldTypes { get; }
    }
}
