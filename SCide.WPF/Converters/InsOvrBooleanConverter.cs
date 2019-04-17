using System;
using System.Globalization;
using System.Windows.Data;

namespace SCide.WPF.Converters
{
    public class InsOvrBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? "Ovr" : "Ins";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // do nothing
            return null;
        }
    }
}
