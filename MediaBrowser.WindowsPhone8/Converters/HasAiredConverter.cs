using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class HasAiredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            var programme = value as ProgramInfoDto;
            if (programme == null)
            {
                return true;
            }

            var endTime = programme.EndDate;
            var now = DateTime.Now;

            return now > endTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
