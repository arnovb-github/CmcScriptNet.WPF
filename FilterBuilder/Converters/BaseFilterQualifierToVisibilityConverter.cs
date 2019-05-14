using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Vovin.CmcLibNet.Database;

namespace CmcScriptNet.FilterBuilder.Converters
{
    [ValueConversion(typeof(FilterQualifier), typeof(Visibility))]
    public abstract class BaseFilterQualifierToVisibilityConverter : IValueConverter
    {
        private static IList<FilterQualifier> qualifiers;

        public BaseFilterQualifierToVisibilityConverter()
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
            if (!(value is FilterQualifier)) { return null; }
            return this.ApplicableQualifiers.Any(a => a.Equals(value)) ? TrueValue : FalseValue;
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

        public abstract IEnumerable<FilterQualifier> ApplicableQualifiers { get; }

        protected internal IEnumerable<FilterQualifier> AllFilterQualifiers 
            { 
            get
            {
                if (qualifiers == null) { qualifiers = new List<FilterQualifier>(); }
                foreach (FilterQualifier fq in Enum.GetValues(typeof(FilterQualifier)))
                {
                    qualifiers.Add(fq);
                }
                return qualifiers;
            }
        }
    }
}

