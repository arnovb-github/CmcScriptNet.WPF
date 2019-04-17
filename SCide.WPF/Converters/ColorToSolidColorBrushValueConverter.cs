using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SCide.WPF.Converters
{
    public class ColorToSolidColorBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return null;
                case System.Windows.Media.Color color:
                    return new SolidColorBrush(color);
                case System.Drawing.Color sdColor:
                    return new SolidColorBrush(System.Windows.Media.Color.FromArgb(sdColor.A, sdColor.R, sdColor.G, sdColor.B));
            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
