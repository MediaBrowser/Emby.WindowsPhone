using System;
using System.Globalization;
using System.Windows.Data;

namespace Emby.WindowsPhone.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public bool ShowTime { get; set; }
        public bool ForLiveTv { get; set; }
        public bool ShowFullDateTime { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime?) value;
            if (!date.HasValue)
            {
                return string.Empty;
            }

            if (ForLiveTv)
            {
                if (ShowFullDateTime)
                {
                    return date.Value.ToLocalTime().ToString(CultureInfo.CurrentUICulture);
                }

                return ShowTime ? date.Value.ToLocalTime().ToShortTimeString() : date.Value.ToLocalTime().ToShortDateString();
            }

            if (ShowFullDateTime)
            {
                return date.Value.ToLocalTime().ToString(CultureInfo.CurrentUICulture);
            }

            return ShowTime ? date.Value.ToShortTimeString() : date.Value.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}