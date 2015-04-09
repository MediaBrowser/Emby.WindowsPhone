using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Emby.WindowsPhone.Converters
{
    public class ZeroCountBrushConverter : IValueConverter
    {
        public SolidColorBrush ZeroCountBrush { get; set; }
        public SolidColorBrush AltBrush { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return ZeroCountBrush;
            }

            var count = int.Parse(value.ToString());

            return count == 0 ? ZeroCountBrush : AltBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}