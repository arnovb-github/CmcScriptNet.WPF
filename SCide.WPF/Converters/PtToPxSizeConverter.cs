using System;
using System.Globalization;
using System.Windows.Data;

namespace SCide.WPF.Converters
{
    public class PtToPxSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return (int)((72.0/96.0) * (double)value); // assuming default settings
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
