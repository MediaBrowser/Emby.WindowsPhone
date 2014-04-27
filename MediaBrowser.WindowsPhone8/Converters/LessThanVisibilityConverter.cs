using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class LessThanVisibilityConverter : IValueConverter
    {
        public bool NullIsVisible { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return NullIsVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            var lessThanValue = double.Parse(parameter.ToString());
            var number = double.Parse(value.ToString());

            return number < lessThanValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GreaterThanVisibilityConverter : IValueConverter
    {
        public bool NullIsVisible { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return NullIsVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            var greaterThanValue = double.Parse(parameter.ToString());
            var number = double.Parse(value.ToString());

            return number > greaterThanValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
