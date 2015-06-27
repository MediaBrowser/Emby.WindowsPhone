using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;

namespace Emby.WindowsPhone.Converters
{
    public class ScheduledRecordingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var schedule = value as TimerInfoDto;
            if (schedule == null)
            {
                return string.Empty;
            }

            return string.Format("{0} - {1}", schedule.StartDate.ToLocalTime().ToShortTimeString(), schedule.EndDate.ToLocalTime().ToShortTimeString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}