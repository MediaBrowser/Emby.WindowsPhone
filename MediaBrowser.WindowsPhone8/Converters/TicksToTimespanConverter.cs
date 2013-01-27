using System;
using System.Windows.Data;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class TicksToTimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var runtimeTicks = (long)value;
                var runtime = TimeSpan.FromTicks(runtimeTicks);
                return runtime;
            }
            return new TimeSpan();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
